using Optern.Application.DTOs.Message;
using Optern.Application.Interfaces.IMessageService;

namespace Optern.Presentation.GraphQlApi.Message.Mutation
{
    [ExtendObjectType("Mutation")]
    public class MessageMutation
    {
        [GraphQLDescription("Send Message To Room")]
        public async Task<Response<MessageDTO>> SendMessageToRoomAsync([Service] IMessageService _messageService ,int chatId, string senderId, string? content = null, IFile? file = null)=>
            await _messageService.SendMessageToRoomAsync(chatId, senderId, content, file);  

    }
}
