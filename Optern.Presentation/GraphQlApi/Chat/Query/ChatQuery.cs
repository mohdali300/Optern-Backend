﻿
namespace Optern.Presentation.GraphQlApi.Chat.Query
{
    [ExtendObjectType("Query")]
    public class ChatQuery
    {
        [GraphQLDescription("Get room chat participants")]
        public async Task<Response<List<ChatParticipants>>> GetChatParticipants([Service]IChatService _chatService, int chatId)
            =>await _chatService.GetChatParticipantsAsync(chatId);
    }
}
