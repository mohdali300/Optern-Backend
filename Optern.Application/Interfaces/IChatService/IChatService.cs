namespace Optern.Application.Interfaces.IChatService
{
    public interface IChatService
    {
        public Task<Response<ChatDTO>> CreateRoomChat(string creatorId, ChatType type);
        public Task<Response<bool>> JoinToRoomChat(string roomId,string participantId);
        public Task<Response<bool>> JoinAllToRoomChat(string roomId,List<string> participantsIds);
        public Task<Response<List<ChatParticipantsDTO>>> GetChatParticipants(int chatId); //for test
    }
}
