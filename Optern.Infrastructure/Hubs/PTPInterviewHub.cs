using Optern.Infrastructure.ExternalServices.CacheService;
using System.Collections.Concurrent;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Task = System.Threading.Tasks.Task;

namespace Optern.Infrastructure.Hubs
{
    public class PTPInterviewHub(IPTPInterviewService pTPInterviewService, IUserService userService, IHubContext<PTPInterviewHub> hubContext
        , ICacheService cacheService) : Hub
    {
        private readonly IPTPInterviewService _pTPInterviewService = pTPInterviewService;
        private readonly IUserService _userService = userService;
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnectionMap = new();
        private readonly IHubContext<PTPInterviewHub> _hubContext = hubContext;
        private static readonly ConcurrentDictionary<int, DateTime> _sessionEndTimeMap = new();
        private static readonly ConcurrentDictionary<int, Timer> _sessionTimers = new();
        private readonly ICacheService _cacheService = cacheService;
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
                var (isValid, interviewStartDateTime) = await ValidateInterviewJoinProcess(sessionId);
                // if (!isValid)
                // {
                //     return;
                // }

                var response = await _pTPInterviewService.StartPTPInterviewSessionAsync(sessionId, userId);
                if (response.IsSuccess)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"ptpInterview{sessionId}");
                    await Clients.Caller.SendAsync("JoinToSession", "Joined to interview session successfully");

                    if (!_sessionEndTimeMap.ContainsKey(sessionId))
                    {
                        StartInterviewTimer(sessionId, interviewStartDateTime);
                    }
                    await BroadcastRemainingTime(sessionId);
                }
                else
                {
                    await Clients.Caller.SendAsync("FailedJoinToSession", "Failed to start interview session");
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("JoinSessionError", "Server error try again");
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
               _cacheService.SetData($"session:{sessionId}:code", code, TimeSpan.FromHours(24));

            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("UpdatedCode", code);
           
        }

        [HubMethodName("CodeOutput")]
        public async Task CodeOutput(int sessionId, string output)
        {
            _cacheService.SetData($"session:{sessionId}:output", output, TimeSpan.FromHours(24));

            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("CodeOutput", output);
        }

        [HubMethodName("SwapRole")]
        public async Task SwapRole(int sessionId, string userId)
        {
            _cacheService.SetData($"session:{sessionId}:SwapRole", userId, TimeSpan.FromHours(24));
            await Clients.OthersInGroup($"ptpInterview{sessionId}").SendAsync("SwapRole", userId);
        }

        [HubMethodName("Language")]
        public async Task Language(int sessionId, string language)
        {
            _cacheService.SetData($"session:{sessionId}:language", language, TimeSpan.FromHours(24));
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

        // Helpers

        #region BroadcastRemainingTime

        private async Task BroadcastRemainingTime(int sessionId)
        {
            if (_sessionEndTimeMap.TryGetValue(sessionId, out var endTime))
            {
                var remainingTime = endTime - DateTime.UtcNow.AddHours(2).ToUniversalTime();

                if (remainingTime.TotalSeconds <= 0)
                {
                    if (_sessionTimers.TryRemove(sessionId, out var timer))
                    {
                        timer.Dispose();
                    }
                    _sessionEndTimeMap.TryRemove(sessionId, out _);
                    await _hubContext.Clients.Group($"ptpInterview{sessionId}").SendAsync("TimerEnded", "The interview session time is over");
                   await EndInterviewSession(sessionId);
                }
                else
                {
                     _cacheService.SetData($"session:{sessionId}:timer", remainingTime.ToString(@"hh\:mm\:ss"), TimeSpan.FromHours(1));
                    await _hubContext.Clients.Group($"ptpInterview{sessionId}").SendAsync("UpdateTimer", remainingTime.ToString(@"hh\:mm\:ss"));
                }
            }
        }

        #endregion BroadcastRemainingTime

        #region Start Timer

        private void StartInterviewTimer(int sessionId, DateTime interviewStartDateTime)
        {
            interviewStartDateTime = DateTime.SpecifyKind(interviewStartDateTime, DateTimeKind.Utc);
            var endTime = interviewStartDateTime.ToUniversalTime().AddHours(1);
            _sessionEndTimeMap[sessionId] = endTime;

            var timer = new Timer(async _ =>
            {
                await BroadcastRemainingTime(sessionId);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            _sessionTimers[sessionId] = timer;
        }

        #endregion Start Timer

        #region Validate Join To Interview Process

        private async Task<(bool isValid, DateTime interviewStartDateTime)> ValidateInterviewJoinProcess(int sessionId)
        {
            var interview = await _pTPInterviewService.GetInterviewTimeSlot(sessionId);
            if (interview == null)
            {
                await Clients.Caller.SendAsync("NotFoundInterview", "This Interview Not Found");
                return (false, DateTime.MinValue);
            }
             
            var interviewTime = _pTPInterviewService.GetTimeSpanFromEnum(interview.Data.ScheduledTime);
            var interviewDate = DateTime.Parse(interview.Data.ScheduledDate).Date;
            var interviewStartDateTime = interviewDate.Add(interviewTime);
            var interviewEndTime = interviewStartDateTime.AddHours(1);
            var currentTime = DateTime.UtcNow.AddHours(2);

            // if (currentTime.Date != interviewDate)
            // {
            //     await Clients.Caller.SendAsync("WrongInterviewDate", $"You can only join on the scheduled day: {interview.Data.ScheduledDate}");
            //     return (false, DateTime.MinValue);
            // }

            // if (currentTime < interviewStartDateTime)
            // {
            //     await Clients.Caller.SendAsync("InterviewNotStarted", $"This Interview Not Started Yet, will start at: {interviewTime}");
            //     return (false, DateTime.MinValue);
            // }

            // if (currentTime > interviewEndTime)
            // {
            //     await Clients.Caller.SendAsync("InterviewEnded", "This Interview Ended");
            //     return (false, DateTime.MinValue);
            // }

            return (true, interviewStartDateTime);
        }

        #endregion Validate Join To Interview Process
    }
}