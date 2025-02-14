using System.Collections.Concurrent;
using Task = System.Threading.Tasks.Task;

namespace Optern.Infrastructure.Hubs
{

    [Authorize]
    public class ChatHub:Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IChatService _chatService;
        private readonly IMessageService _messageService;
        private static readonly ConcurrentDictionary<string, string> _userConnectionMap = new();

        public ChatHub(ILogger<ChatHub> logger, IUnitOfWork unitOfWork, IUserService userService, IChatService chatService,IMessageService messageService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _chatService = chatService;
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            var user=await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                var userId = user.Id;
                _userConnectionMap[userId] = Context.ConnectionId; // track user connections (from more than a device)

                var userChats = await _chatService.GetChatParticipantsAsync(chatId:null,userId);
                foreach (var chat in userChats.Data)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"{chat.Chat.Room.Name}{chat.ChatId}");
                    _logger.LogInformation($"{user.FirstName} {user.LastName} reconnect to {chat.Chat.Room.Name}{chat.ChatId}.");
                }

                // send all unread messages

            }

            await base.OnConnectedAsync();
        }

        [HubMethodName("jointoroomchat")]
        public async Task JoinToRoomChat(string roomName,int chatId,string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{roomName}{chatId}");
            await Clients.OthersInGroup($"{roomName}{chatId}").SendAsync("New Member", $"{userName} joined to Room.");
            _logger.LogInformation($"{userName} joined to {roomName}{chatId} group.");
        }

        [HubMethodName("leaveroomchat")]
        public async Task LeaveRoomChat(string roomName, int chatId, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{roomName}{chatId}");
            await Clients.OthersInGroup($"{roomName}{chatId}").SendAsync("Member Left", $"{userName} left the Room.");
            _logger.LogInformation($"{userName} left the {roomName}{chatId} group.");
        }

        [HubMethodName("SendMessageToRoom")]
        public async Task SendMessageToRoom(string roomName,int chatId, string senderId, string? content = null, IFile? file = null)
        {
            //var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var messageResult = await _messageService.SendMessageToRoomAsync(chatId,senderId,content,file);

            if (messageResult.IsSuccess)
            {
                await Clients.Group($"{roomName}{chatId}").SendAsync("ReceiveMessage", messageResult.Data);
            }
        }
        [HubMethodName("DeleteMessage")]
        public async Task DeleteMessage(string roomName, int messageId)
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    await Clients.Caller.SendAsync("DeleteMessageFailed", "Unauthorized");
                    return;
                }

                var result = await _messageService.DeleteMessageAsync(messageId, userId);

                if (result.IsSuccess)
                {
                    await Clients.Group($"{roomName}{result.Data}").SendAsync("MessageDeleted", messageId);

                    await Clients.Caller.SendAsync("MessageDeleted", messageId);
                }
                else
                {
                    await Clients.Caller.SendAsync("DeleteMessageFailed", result.Errors);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("DeleteMessageFailed", "Internal server error");
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user=await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                _userConnectionMap.TryRemove(user.Id, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
