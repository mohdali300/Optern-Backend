using Optern.Application.DTOs.Message;
using Optern.Application.Interfaces.IMessageService;
using Optern.Domain.Entities;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using Optern.Infrastructure.ExternalServices.FileService;

namespace Optern.Application.Services.MessageService
{
    public class MessageService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper,ICloudinaryService cloudinaryService) :IMessageService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
        public async Task<Response<MessageDTO>> SendMessageToRoomAsync(int chatId, string senderId, string content, IFile? file = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var chat = await _context.Chats
                    .Include(c => c.ChatParticipants)
                    .FirstOrDefaultAsync(c => c.Id == chatId);

                if (chat == null)
                    return Response<MessageDTO>.Failure(new MessageDTO(), "Chat Room not found.", 404);

                if (!chat.ChatParticipants.Any(p => p.UserId == senderId))
                    return Response<MessageDTO>.Failure(new MessageDTO(), "User not in room chat.", 403);

                string? fileUrl = null;
                if (file != null)
                {
                    try
                    {
                        var uploadResult = await _cloudinaryService.UploadFileAsync(file, "chat-attachments");

                        if (string.IsNullOrEmpty(uploadResult.Url))
                        {
                            return Response<MessageDTO>.Failure(new MessageDTO(), "Failed to upload file. Please try again.", 500);
                        }

                        fileUrl = uploadResult.Url;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return Response<MessageDTO>.Failure(new MessageDTO(), $"File upload error: {ex.Message}", 500);
                    }
                }

                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = senderId,
                    Content = content,
                    SentAt = DateTime.UtcNow,
                   // AttachmentUrl = fileUrl
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var messageDto = _mapper.Map<MessageDTO>(message);
                return Response<MessageDTO>.Success(messageDto, "Message sent successfully.", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<MessageDTO>.Failure(new MessageDTO(), $"An error occurred while sending the message: {ex.Message}", 500);
            }
        }

    }
}
