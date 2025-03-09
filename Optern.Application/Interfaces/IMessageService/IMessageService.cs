using Optern.Application.DTOs.Message;

namespace Optern.Application.Interfaces.IMessageService
{
    public interface IMessageService
    {
        public Task<Response<MessageDTO>> SendMessageAsync(int chatId, string senderId, string? content = null, IFile? file = null);
        public Task<Response<bool>> DeleteMessageAsync(int messageId, string userId);
        public Task<Response<List<MessageDTO>>> GetChatMessagesAsync(int chatId);
        public Task<Response<List<MessageDTO>>> GetUnreadMessagesAsync(int chatId);



    }
}
