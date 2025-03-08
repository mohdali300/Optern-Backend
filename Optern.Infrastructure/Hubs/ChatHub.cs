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
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnectionMap = new(); // track user connections (from more than a device)

        public ChatHub(ILogger<ChatHub> logger, IUnitOfWork unitOfWork, IUserService userService, IChatService chatService, IMessageService messageService)
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
                _userConnectionMap.AddOrUpdate(
                    userId, // key
                    new HashSet<string> { Context.ConnectionId }, // value if key doesnt exist
                    (key, connections) => // if key exists, add current ConId to its connections
                    {
                        lock (connections)
                        {
                            connections.Add(Context.ConnectionId);
                            return connections;
                        }
                    }
               );

                _logger.LogInformation($"{user.FirstName} {user.LastName} reconnect with ConnectionId: {Context.ConnectionId}.");
                var userChats = await _chatService.GetChatParticipantsAsync(null,userId);
                foreach (var chat in userChats.Data)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, chat.Chat.Room.Id);
                    await GetUnreadMessages(chat.Id);
                }
            }

            await base.OnConnectedAsync();
        }

        [HubMethodName("JoinToRoomChat")]
        public async Task JoinToRoomChat(string roomId)
        {
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "New Collaborator";
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.OthersInGroup(roomId).SendAsync("New Member", $"{userName} joined to Room.");
            _logger.LogInformation($"{userName} joined to {roomId} room.");
        }

        [HubMethodName("LeaveRoomChat")]
        public async Task LeaveRoomChat(string roomId)
        {
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Collaborator";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.OthersInGroup(roomId).SendAsync("Member Left", $"{userName} left the Room.");
            _logger.LogInformation($"{userName} left the {roomId} room.");
        }

        [HubMethodName("SendMessageToRoom")]
        public async Task SendMessageToRoom(string roomId, int chatId, string userId, string? content = null, IFile? file = null)
        {
            try
            {
                //var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //if (userId == null)
                //{
                //    throw new HubException("Unauthorized: User not found.");
                //}

                var messageResult = await _messageService.SendMessageAsync(chatId, userId, content, file);

                if (messageResult.IsSuccess)
                {
                    await Clients.Group(roomId).SendAsync("ReceiveMessage", messageResult.Data);
                }
                else
                {
                    await Clients.Caller.SendAsync("ReceiveError", messageResult.Message);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveError", "An unexpected error occurred. Please try again.");
            }
        }
        [HubMethodName("DeleteMessage")]
        public async Task DeleteMessage(string roomId, int messageId, string userId)
        {
            try
            {
                //var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //if (string.IsNullOrEmpty(userId))
                //{
                //    await Clients.Caller.SendAsync("DeleteMessageFailed", "Unauthorized");
                //    return;
                //}

                var result = await _messageService.DeleteMessageAsync(messageId, userId);

                if (result.IsSuccess)
                {
                    await Clients.Group(roomId).SendAsync("MessageDeleted", messageId);

                    //await Clients.Caller.SendAsync("MessageDeleted", messageId);
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

        [HubMethodName("GetChatMessages")]
        public async Task GetChatMessages(int chatId)
        {
            try
            {
                var result = await _messageService.GetChatMessagesAsync(chatId);
                if (result.IsSuccess)
                {
                    await Clients.Caller.SendAsync("ReceiveChatMessages", result.Data, result.Message);
                }
                else
                {
                    await Clients.Caller.SendAsync("ReceiveError", result.Message);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveError", "An unexpected error occurred while retrieving chat messages.");
            }
        }

        [HubMethodName("GetUnreadMessages")]
        public async Task GetUnreadMessages(int chatId)
        {
            try
            {
                var result = await _messageService.GetUnreadMessagesAsync(chatId);
                if (result.IsSuccess)
                {
                    await Clients.Caller.SendAsync("ReceiveUnreadMessages", result.Data, result.Message);
                }
                else
                {
                    await Clients.Caller.SendAsync("ReceiveError", result.Message);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveError", "An unexpected error occurred while retrieving unread messages.");
            }
        }
    
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user=await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                if (_userConnectionMap.TryGetValue(user.Id,out var userConnections))
                {
                    lock (userConnections)
                    {
                        userConnections.Remove(Context.ConnectionId);
                        if (userConnections.Count == 0)
                            _userConnectionMap.TryRemove(user.Id, out _);
                        _logger.LogInformation($"{user.FirstName} {user.LastName} disconnect from ConnectionId: {Context.ConnectionId}.");
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
