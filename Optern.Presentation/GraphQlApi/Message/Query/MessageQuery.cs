using Optern.Application.DTOs.Message;
using Optern.Application.Interfaces.IMessageService;

namespace Optern.Presentation.GraphQlApi.Message.Query
{
    [ExtendObjectType("Query")]

    public class MessageQuery
    {
        [GraphQLDescription("Get Chat Messages")]
        public async Task<Response<List<MessageDTO>>> GetChatMessagesAsync([Service] IMessageService _messageService,int chatId)=>
            await _messageService.GetChatMessagesAsync(chatId);

        [GraphQLDescription("Get Unread Messages")]
        public async Task<Response<List<MessageDTO>>> GetUnreadMessagesAsync([Service] IMessageService _messageService, int chatId) =>
            await _messageService.GetUnreadMessagesAsync(chatId);


    }
}
