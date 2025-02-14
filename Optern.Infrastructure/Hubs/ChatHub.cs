using Optern.Domain.Entities;
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
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnectionMap = new(); // track user connections (from more than a device)

        public ChatHub(ILogger<ChatHub> logger, IUnitOfWork unitOfWork, IUserService userService, IChatService chatService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _chatService = chatService;
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

                var userChats = await _chatService.GetChatParticipantsAsync(null,userId);
                foreach (var chat in userChats.Data)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"{chat.Chat.Room.Id}");
                    _logger.LogInformation($"{user.FirstName} {user.LastName} reconnect to {chat.Chat.Room.Id} with ConnectionId: {Context.ConnectionId}.");
                }

                // send all unread messages

            }

            await base.OnConnectedAsync();
        }

        [HubMethodName("jointoroomchat")]
        public async Task JoinToRoomChat(string roomId)
        {
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "New Collaborator";
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{roomId}");
            await Clients.OthersInGroup($"{roomId}").SendAsync("New Member", $"{userName} joined to Room.");
            _logger.LogInformation($"{userName} joined to {roomId} room.");
        }

        [HubMethodName("leaveroomchat")]
        public async Task LeaveRoomChat(string roomId)
        {
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Collaborator";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{roomId}");
            await Clients.OthersInGroup($"{roomId}").SendAsync("Member Left", $"{userName} left the Room.");
            _logger.LogInformation($"{userName} left the {roomId} room.");
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
