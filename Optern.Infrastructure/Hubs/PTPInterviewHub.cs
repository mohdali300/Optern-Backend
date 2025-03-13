using System.Collections.Concurrent;
using Task = System.Threading.Tasks.Task;

namespace Optern.Infrastructure.Hubs
{
    public class PTPInterviewHub(IPTPInterviewService pTPInterviewService, IUserService userService, IHubContext<PTPInterviewHub> hubContext) : Hub
    {
        private readonly IPTPInterviewService _pTPInterviewService = pTPInterviewService;
        private readonly IUserService _userService = userService;
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnectionMap = new();
        private readonly IHubContext<PTPInterviewHub> _hubContext = hubContext;
        private static readonly ConcurrentDictionary<int, DateTime> _sessionEndTimeMap = new();
        private static readonly ConcurrentDictionary<int, Timer> _sessionTimers = new();

        public override async Task OnConnectedAsync()
        {
            try
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

                    var currentInterviewResponse = await _pTPInterviewService.GetUserCurrentPTPInterviewSessionAsync(userId);
                    if (currentInterviewResponse.IsSuccess && currentInterviewResponse.StatusCode == 200)
                    {
                        int sessionId = currentInterviewResponse.Data.Id;
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"ptpInterview{sessionId}");
                        await Clients.Caller.SendAsync("JoinToSession", "Joined to interview session successfully.");

                        await BroadcastRemainingTime(sessionId);
                    }
                }
                await base.OnConnectedAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Clients.Caller.SendAsync("JoinToSession", e.Message);
            }
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

                    if (!_sessionEndTimeMap.ContainsKey(sessionId))
                    {
                        var endTime = DateTime.UtcNow.AddHours(1);
                        _sessionEndTimeMap[sessionId] = endTime;

                        var timer = new Timer(async _ =>
                        {
                            await BroadcastRemainingTime(sessionId);
                        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

                        _sessionTimers[sessionId] = timer;
                    }

                    await BroadcastRemainingTime(sessionId);
                }
                else
                {
                    await Clients.Caller.SendAsync("FaliedJoinToSession", "Failed to start interview session.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Clients.Caller.SendAsync("JoinSessionError", "Server error try again");
            }
        }

        private async Task BroadcastRemainingTime(int sessionId)
        {
            if (_sessionEndTimeMap.TryGetValue(sessionId, out var endTime))
            {
                var remainingTime = endTime - DateTime.UtcNow;

                if (remainingTime.TotalSeconds <= 0)
                {
                    if (_sessionTimers.TryRemove(sessionId, out var timer))
                    {
                        timer.Dispose();
                    }
                    _sessionEndTimeMap.TryRemove(sessionId, out _);
                    await _hubContext.Clients.Group($"ptpInterview{sessionId}").SendAsync("TimerEnded", "The interview session time is over.");
                }
                else
                {
                    await _hubContext.Clients.Group($"ptpInterview{sessionId}").SendAsync("UpdateTimer", remainingTime.ToString(@"hh\:mm\:ss"));
                }
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
                    await Clients.Group($"ptpInterview{sessionId}").SendAsync("EndSession", "Interview session ended.");
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

        [HubMethodName("CancelInterviewSession")]
        public async Task CancelInterviewSession(int sessionId, string userId)
        {
            try
            {
                var response = await _pTPInterviewService.CancelPTPInterviewAsync(sessionId, userId);
                if (response.IsSuccess)
                {
                    await Clients.Caller.SendAsync("CancelledSession", "Interview session Cancelled.");
                }
                else
                {
                    await Clients.Caller.SendAsync("FailedCancelSession", "Failed to Cancel Interview session.");
                }
            }
            catch
            {
                await Clients.Caller.SendAsync("CancelSessionError", "Unexpected error occurred while Canceling session.");
            }
        }

        [HubMethodName("UpdateCode")]
        public async Task UpdateCode(int sessionId, string code)
        {
            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("UpdatedCode", code);
        }

        [HubMethodName("CodeOutput")]
        public async Task CodeOutput(int sessionId, string output)
        {
            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("CodeOutput", output);
        }

        [HubMethodName("SwapRole")]
        public async Task SwapRole(int sessionId, string userId)
        {
            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("SwapRole", userId);
        }

        [HubMethodName("Language")]
        public async Task Language(int sessionId, string language)
        {
            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("ReceiveLanguage", language);
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