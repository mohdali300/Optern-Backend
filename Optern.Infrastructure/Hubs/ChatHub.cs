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
        private readonly IRoomUserService _roomUserService;
        private readonly IRoomSettingService _roomSettingService;
        private static readonly ConcurrentDictionary<string, string> _userConnectionMap = new();

        public ChatHub(ILogger<ChatHub> logger, IUnitOfWork unitOfWork, IUserService userService, IChatService chatService, IRoomUserService roomUserService, IRoomSettingService roomSettingService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _chatService = chatService;
            _roomUserService = roomUserService;
            _roomSettingService = roomSettingService;
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
        public async Task<Response<List<RoomUserDTO>>> JoinToRoomChat(string roomId, string leaderId, int? userRoomId = null, bool? approveAll = null)
        {
            var response=await _roomUserService.AcceptRequestsAsync(roomId,leaderId,userRoomId,approveAll);
            if (response.IsSuccess)
            {
                foreach(var user in response.Data)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"{roomId}");
                    await Clients.OthersInGroup($"{roomId}").SendAsync("New Member", $"{user.UserName} joined to Room.");
                    _logger.LogInformation($"{user.UserName} joined to {roomId} room.");
                }
                return Response<List<RoomUserDTO>>.Success(response.Data,response.Message);
            }
            return Response<List<RoomUserDTO>>.Failure(response.Data, response.Message, response.StatusCode);
        }

        [HubMethodName("leaveroomchat")]
        public async Task<Response<bool>> LeaveRoomChat(string roomId, string userId)
        {
            var response=await _roomSettingService.LeaveRoomAsync(roomId, userId);
            if (response.IsSuccess)
            {
                var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{roomId}");
                await Clients.OthersInGroup($"{roomId}").SendAsync("Member Left", $"{userName} left the Room.");
                _logger.LogInformation($"{userName} left the {roomId} room.");

                return Response<bool>.Success(response.Data,response.Message,response.StatusCode);
            }
            return Response<bool>.Failure(response.Data, response.Message, response.StatusCode);
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
