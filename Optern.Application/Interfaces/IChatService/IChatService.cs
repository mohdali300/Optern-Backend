namespace Optern.Application.Interfaces.IChatService
{
    public interface IChatService
    {
        public Task<Response<ChatDTO>> CreatePrivateChatAsync(string creatorId, string receiverId);
        public Task<Response<ChatDTO>> CreateRoomChatAsync(string creatorId, ChatType type);
        public Task<Response<bool>> JoinToRoomChatAsync(string roomId,string participantId);
        public Task<Response<bool>> JoinAllToRoomChatAsync(string roomId,List<string> participantsIds);
        public Task<Response<bool>> RemoveFromRoomChatAsync(int chatId,string userId);
        public Task<Response<List<ChatParticipants>>> GetChatParticipantsAsync(int? chatId=null,string? userId=null);
    }
}
