using System.Collections.Concurrent;
using Task = System.Threading.Tasks.Task;

namespace Optern.Infrastructure.Hubs
{
    public class PTPInterviewHub(IPTPInterviewService pTPInterviewService, IUserService userService) : Hub
    {
        private readonly IPTPInterviewService _pTPInterviewService = pTPInterviewService;
        private readonly IUserService _userService = userService;
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnectionMap = new(); // track user connections (from more than a device)

        public override async Task OnConnectedAsync()
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                var userId = user.Id;
                _userConnectionMap.AddOrUpdate(
                    userId,
                    new HashSet<string> { Context.ConnectionId },
                    (key, connections) =>
                    {
                        lock (connections)
                        {
                            connections.Add(Context.ConnectionId);
                            return connections;
                        }
                    }
               );
                // add user to running interview if he reconnect or connected from another device
                var currentInterviewResponse = await _pTPInterviewService.GetUserCurrentPTPInterviewSessionAsync(userId);
                if (currentInterviewResponse.IsSuccess && currentInterviewResponse.StatusCode == 200)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"ptpInterview{currentInterviewResponse.Data.Id}");
                    await Clients.Caller.SendAsync("JoinToSession", "Joined to interview session successfully.");
                }
            }
            await base.OnConnectedAsync();
        }

        [HubMethodName("JoinInterviewSession")]
        public async Task JoinInterviewSession(int sessionId, string userId)
        {
            try
            {
                var response = await _pTPInterviewService.StartPTPInterviewSessionAsync(sessionId, userId);
                if (response.IsSuccess)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"ptpInterview{sessionId}");
                    await Clients.Caller.SendAsync("JoinToSession", "Joined to interview session successfully.");
                }
                else
                {
                    await Clients.Caller.SendAsync("FaliedJoinToSession", "Failed to start interview session.");
                }
            }
            catch
            {
                await Clients.Caller.SendAsync("JoinSessionError", "Unexpected error occurred while starting the session.");
            }
        }

        [HubMethodName("EndInterviewSession")]
        public async Task EndInterviewSession(int sessionId)
        {
            try
            {
                var response = await _pTPInterviewService.EndPTPInterviewSessionAsync(sessionId);
                if (response.IsSuccess)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ptpInterview{sessionId}");
                    await Clients.Caller.SendAsync("EndSession", "Interview session ended.");
                }
                else
                {
                    await Clients.Caller.SendAsync("FailedEndSession", "Failed to end Interview session.");
                }
            }
            catch
            {
                await Clients.Caller.SendAsync("EndSessionError", "Unexpected error occurred while ending session.");
            }
        }

        [HubMethodName("UpdateCode")]
        public async Task UpdateCode(int sessionId, string code)
        {
            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("UpdatedCode", code);
        }

        [HubMethodName("Language")]
        public async Task Language(int sessionId, string language)
        {
            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("ReceiveLanguage", language);
        }

        [HubMethodName("CodeOutput")]
        public async Task CodeOutput(int sessionId, string output)
        {
            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("CodeOutput", output);
        }

        public async Task SwapRole(int sessionId, string userId)
        {
            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("SwapRole", userId);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                if (_userConnectionMap.TryGetValue(user.Id, out var userConnections))
                {
                    lock (userConnections)
                    {
                        userConnections.Remove(Context.ConnectionId);
                        if (userConnections.Count == 0)
                            _userConnectionMap.TryRemove(user.Id, out _);
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
