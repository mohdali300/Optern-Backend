
namespace Optern.Infrastructure.Services.MessageService
{
    public class MessageService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper,ICloudinaryService cloudinaryService) :IMessageService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

        #region Send Message To Room
        public async Task<Response<MessageDTO>> SendMessageToRoomAsync(int chatId,string senderId,string? content = null,IFile? file = null)
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
                    return Response<MessageDTO>.Failure(new MessageDTO(), "Chat room not found.", 404);
                }

                if (!chat.ChatParticipants.Any(p => p.UserId == senderId))
                {
                    return Response<MessageDTO>.Failure(new MessageDTO(), "You are not a member of this chat room.", 403);
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
                    };

                    _context.Messages.Add(message);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var messageDto = _mapper.Map<MessageDTO>(message);
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

    }
}
