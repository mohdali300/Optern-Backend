
using MimeKit;
using Optern.Application.DTOs.Message;
using Optern.Domain.Entities;
using static HotChocolate.Language.Utf8GraphQLParser;

namespace Optern.Infrastructure.Services.MessageService
{
    public class MessageService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper,ICloudinaryService cloudinaryService, ICacheService cacheService) :IMessageService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
        private readonly ICacheService _cacheService = cacheService;


        #region Send Message
        public async Task<Response<MessageDTO>> SendMessageAsync(int chatId, string senderId, string? content = null, IFile? file = null)
        {
            try
            {
                content = !string.IsNullOrWhiteSpace(content) ? content.Trim() : null;

                if (file == null && content == null)
                {
                    return Response<MessageDTO>.Failure(new MessageDTO(), "Either message content or file attachment is required.", 400);
                }

                var chat = await _context.Chats
                    .Include(c => c.ChatParticipants)
                    .FirstOrDefaultAsync(c => c.Id == chatId);

                if (chat == null)
                {
                    return Response<MessageDTO>.Failure(new MessageDTO(), "Chat not found.", 404);
                }

                if (!chat.ChatParticipants.Any(p => p.UserId == senderId))
                {
                    return Response<MessageDTO>.Failure(new MessageDTO(), "You are not a member of this chat.", 403);
                }

                string? fileUrl = null;
                string? publicId = null;

                if (file != null)
                {
                    if (file.Length > 10_485_760)
                    {
                        return Response<MessageDTO>.Failure(new MessageDTO(), "File size exceeds maximum allowed 10MB.", 400);
                    }

                    try
                    {
                        (publicId, fileUrl) = await _cloudinaryService.UploadFileAsync(file, "chat-attachments");

                        if (string.IsNullOrEmpty(fileUrl))
                        {
                            return Response<MessageDTO>.Failure(new MessageDTO(), "Failed to upload file. Please try again.", 500);
                        }
                    }
                    catch (Exception uploadEx)
                    {
                        return Response<MessageDTO>.Failure(new MessageDTO(), "File upload service unavailable. Please try again later.", 503);
                    }
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var message = new Message
                    {
                        ChatId = chatId,
                        SenderId = senderId,
                        Content = content,
                        SentAt = DateTime.UtcNow,
                        AttachmentUrl = fileUrl,
                        IsDeleted = false,
                        Chat = chat,
                    };

                    _context.Messages.Add(message);
                    await _context.SaveChangesAsync();

                    var messageDto = _mapper.Map<MessageDTO>(message);

                    string cacheKey = $"chat_{chatId}_messages";
                    var cachedMessages = _cacheService.GetData<List<MessageDTO>>(cacheKey) ?? new List<MessageDTO>();
                    cachedMessages.Add(messageDto);
                    _cacheService.SetData(cacheKey, cachedMessages, TimeSpan.FromMinutes(10));

                    await transaction.CommitAsync();

                    return Response<MessageDTO>.Success(
                        messageDto,
                        file != null && content != null ? "Message with attachment sent successfully"
                        : file != null ? "File sent successfully"
                        : "Message sent successfully",
                        200
                    );
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    if (!string.IsNullOrEmpty(publicId))
                    {
                        await _cloudinaryService.DeleteFileAsync(publicId);
                    }

                    return Response<MessageDTO>.Failure(new MessageDTO(), "Failed to send message. Please try again.", 500);
                }
            }
            catch (Exception ex)
            {
                return Response<MessageDTO>.Failure(new MessageDTO(), "An unexpected error occurred. Please try again.", 500);
            }
        }


        #endregion

        #region Delete Message From Room

        public async Task<Response<bool>> DeleteMessageAsync(int messageId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var message = await _context.Messages.FindAsync(messageId);

                if (message == null)
                {
                    return Response<bool>.Failure(false,"Message not found.", 404);
                }
                if (message.SenderId != userId)
                {
                    return Response<bool>.Failure(false, "You can only delete your own messages.", 403);
                }
                if (message.IsDeleted)
                {
                    return Response<bool>.Failure(false, "The message already Deleted", 404);
                }

                if (!string.IsNullOrEmpty(message.AttachmentUrl))
                {
                    try
                    {
                        string publicId = ExtractPublicIdFromUrl(message.AttachmentUrl);
                        var fileDeleted =await _cloudinaryService.DeleteFileAsync(publicId);
                        if (!fileDeleted)
                        {
                            return Response<bool>.Failure(false, "Failed to delete file from cloud storage.", 500);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Response<bool>.Failure(false,"Failed to delete file from cloud storage.", 500);
                    }
                }

                message.IsDeleted = true;
                _context.Messages.Update(message);

                await _context.SaveChangesAsync();
                await _cacheService.RemoveDataAsync($"chat_{message.ChatId}_messages");
                await transaction.CommitAsync();

                return Response<bool>.Success(true,"Message deleted successfully.", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false,"Failed to delete message. Please try again.", 500);
            }
        }


        #endregion

        #region Get All Chat Messages
        public async Task<Response<List<MessageDTO>>> GetChatMessagesAsync(int chatId)
        {
            try
            {
                string cacheKey = $"chat_{chatId}_messages";

                var cachedMessages = _cacheService.GetData<List<MessageDTO>>(cacheKey);
                if (cachedMessages != null)
                {
                    return Response<List<MessageDTO>>.Success(cachedMessages, "Messages retrieved from cache.", 200);
                }

                var messages = await _context.Messages
                    .Include(m => m.Sender)
                    .Where(m => m.ChatId == chatId && !m.IsDeleted)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();
                if (!messages.Any())
                {
                    return Response<List<MessageDTO>>.Success(new List<MessageDTO>(), "No messages found", 200);
                }

                var messageDTOs = _mapper.Map<List<MessageDTO>>(messages);

                _cacheService.SetData(cacheKey, messageDTOs, TimeSpan.FromMinutes(10));

                return Response<List<MessageDTO>>.Success(messageDTOs, "Messages retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<List<MessageDTO>>.Failure(new List<MessageDTO>(), $"An error occurred while retrieving messages: {ex.Message}", 500);
            }
        }

        #endregion

        #region Get Unread Messages
        public async Task<Response<List<MessageDTO>>> GetUnreadMessagesAsync(int chatId)
        {
            try
            {
                //string cacheKey = $"chat_{chatId}_unread_messages";

                //var cachedMessages = _cacheService.GetData<List<MessageDTO>>(cacheKey);
                //if (cachedMessages != null)
                //{
                //    return Response<List<MessageDTO>>.Success(cachedMessages, "Unread messages retrieved from cache.", 200);
                //}

                var messages = await _context.Messages
                    .Include(m => m.Sender)
                    .Where(m => m.ChatId == chatId && !m.IsDeleted && !m.IsRead)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();

                if (!messages.Any())
                {
                    return Response<List<MessageDTO>>.Success(new List<MessageDTO>(), "No unread messages found.", 200);
                }

                messages.ForEach(m => m.IsRead = true);
                var messageDTOs = _mapper.Map<List<MessageDTO>>(messages);

                //_cacheService.SetData(cacheKey, messageDTOs, TimeSpan.FromMinutes(1));
                
                await _context.SaveChangesAsync();

                return Response<List<MessageDTO>>.Success(messageDTOs, "Unread messages retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<List<MessageDTO>>.Failure(new List<MessageDTO>(), $"An error occurred while retrieving unread messages: {ex.Message}", 500);
            }
        }

        #endregion

        #region helpers
        private string ExtractPublicIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');

            var uploadIndex = Array.IndexOf(segments, "upload");
            if (uploadIndex == -1 || uploadIndex + 1 >= segments.Length)
            {
                throw new Exception("Invalid Cloudinary URL format.");
            }

            string publicIdWithExtension = string.Join("/", segments.Skip(uploadIndex + 1));
            string publicId = Path.ChangeExtension(publicIdWithExtension, null); // Remove extension
            return publicId;
        }


        #endregion

    }
}

