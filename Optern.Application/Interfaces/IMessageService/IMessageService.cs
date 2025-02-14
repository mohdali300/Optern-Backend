using Optern.Application.DTOs.Message;

namespace Optern.Application.Interfaces.IMessageService
{
    public interface IMessageService
    {
        public Task<Response<MessageDTO>> SendMessageToRoomAsync(int chatId, string senderId, string? content = null, IFile? file = null);
        public Task<Response<int>> DeleteMessageAsync(int messageId, string userId);
        public Task<Response<List<MessageDTO>>> GetChatMessagesAsync(int chatId);


    }
}
