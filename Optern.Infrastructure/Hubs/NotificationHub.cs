using Optern.Application.DTOs.Notification;
using Optern.Application.DTOs.UserNotification;
using Optern.Application.Interfaces.IUserNotificationService;
using Optern.Infrastructure.Services.UserService;
using System.Collections.Concurrent;
using Task = System.Threading.Tasks.Task;

namespace Optern.Infrastructure.Hubs
{
     [Authorize]
    public class NotificationHub(INotificationService notificationService, IUserNotificationService userNotificationService, IUserService userService) : Hub
    {
        private readonly INotificationService _notificationService= notificationService;
        private readonly IUserNotificationService _userNotificationService= userNotificationService;
        private readonly IUserService _userService = userService;

        private static ConcurrentDictionary<string, HashSet<string>> userConnections = new ConcurrentDictionary<string, HashSet<string>>();
        private static ConcurrentDictionary<string, string> userRooms = new ConcurrentDictionary<string, string>();



        #region On Users Connected

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                userConnections.AddOrUpdate(
                    userId,
                    new HashSet<string> { Context.ConnectionId },
                    (key, oldValue) =>
                    {
                        oldValue.Add(Context.ConnectionId);
                        return oldValue;
                    });
            }
            await base.OnConnectedAsync();
        }
        #endregion


        #region On Users DisConnected
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = userConnections.FirstOrDefault(x => x.Value.Contains(Context.ConnectionId)).Key;

            if (!string.IsNullOrEmpty(userId))
            {
                if (userConnections.TryGetValue(userId, out var connections))
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        userConnections.TryRemove(userId, out _);
                    }
                }

                if (userRooms.TryGetValue(userId, out var roomId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
                    userRooms.TryRemove(userId, out _);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
        #endregion


        #region Join To Room Notifications
        public async Task<Response<bool>> JoinRoom(string roomId)
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];

            if (string.IsNullOrEmpty(userId))
            {
                return Response<bool>.Failure(false, "UserId Not Found", 404);
            }

            if (userRooms.ContainsKey(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRooms[userId]);
                userRooms[userId] = roomId;
            }
            else
            {
                userRooms.TryAdd(userId, roomId);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            return Response<bool>.Success(true, "User Joined To Room Successfully", 200);
        } 
        #endregion


        #region Leave RoomNotifications

        public async Task<Response<bool>> LeaveRoom()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];
            if (!string.IsNullOrEmpty(userId) && userRooms.TryGetValue(userId, out var roomId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
                userRooms.TryRemove(userId, out _);
                return Response<bool>.Success(true, "User Leaved From Room Successfully", 200);
            }
            return Response<bool>.Failure(false, "UserId Not Found", 404);

        }

        #endregion


        #region Send Notifications To All Users in System
        public async Task<Response<bool>> SendNotificationToAll(string? title, string message,string Url)
        {
            if (string.IsNullOrEmpty(message))
            {
                return Response<bool>.Failure(false,"message should not be empty", 400);

            }
            try
            {
                var notification = new NotificationDTO()
                {
                    Title = title,
                    Message = message,
                    Url = Url
                };

                var notificationResult = await _notificationService.AddNotification(notification);
                if (!notificationResult.IsSuccess)
                {
                    return Response<bool>.Failure("Failed to save notification", 500);
                }

                var users = await _userService.GetAll();
                foreach (var user in users.Data)
                {
                    var userNotification = new UserNotificationDTO()
                    {
                        UserId = user.Id,
                        NotificationId = notificationResult.Data.Id
                    };
                    await _userNotificationService.SaveNotification(userNotification);
                }
                await Clients.All.SendAsync("ReceiveNotificationAll", title, message, DateTime.UtcNow);
                return Response<bool>.Success(true, "Notification sent successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure("An error occurred while sending the notification", 500);
            }
        }

        #endregion


        #region Send Notification To Specific User in System
        public async Task<Response<bool>> SendNotificationToUser(string userId, string? title, string message,string Url)
        {
            try
            {
                var notification = new NotificationDTO()
                {
                    Title = title,
                    Message = message,
                    Url = Url
                };

                var notificationResult = await _notificationService.AddNotification(notification);
                if (!notificationResult.IsSuccess)
                {
                    return Response<bool>.Failure("Failed to save notification", 500);
                }

                var userNotification = new UserNotificationDTO()
                {
                    UserId = userId,
                    NotificationId = notificationResult.Data.Id
                };
                var userNotificationResult = await _userNotificationService.SaveNotification(userNotification);

                if (userNotificationResult.IsSuccess && userConnections.TryGetValue(userId, out var connections))
                {
                    foreach (var connectionId in connections)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveNotificationUser", title, message, DateTime.UtcNow);
                    }
                }
                return Response<bool>.Success(true, "Notification sent successfully", 200);

            }
            catch (Exception ex)
            {
                return Response<bool>.Failure("An error occurred while sending the notification", 500);

            }
        }
        #endregion


        #region Send Notification To All in Room
        public async Task<Response<bool>> SendNotificationToAllInRoom(string roomId, string? title, string message,string Url)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(roomId))
            {
                return Response<bool>.Failure(false, "Invalid Data", 400);
            }

            try
            {
                var notification = new NotificationDTO()
                {
                    Title = title,
                    Message = message,
                    RoomId = roomId,
                    Url = Url
                };

                var notificationResult = await _notificationService.AddNotification(notification);
                if (!notificationResult.IsSuccess)
                {
                    return Response<bool>.Failure("Failed to save notification", 500);
                }

                var usersInRoom = userRooms.Where(x => x.Value == roomId).Select(x => x.Key).ToList();

                foreach (var userId in usersInRoom)
                {
                    var userNotification = new UserNotificationDTO()
                    {
                        UserId = userId,
                        NotificationId = notificationResult.Data.Id
                    };
                    await _userNotificationService.SaveNotification(userNotification);
                }

                await Clients.Group(roomId).SendAsync("ReceiveNotificationRoom", title, message, DateTime.UtcNow);

                return Response<bool>.Success(true, "Notification sent successfully", 200);
            }
            catch (Exception)
            {
                return Response<bool>.Failure("An error occurred while sending the notification", 500);
            }
        } 
        #endregion


        #region Mark Noatification as Read for Specific User
        public async Task<Response<bool>> MarkNotificationAsRead(string userId, int notificationId)
        {
            if (string.IsNullOrWhiteSpace(userId) || notificationId <= 0)
                return Response<bool>.Failure("Invalid parameters provided.", 400);
            try
            {
                var notification = new UserNotificationDTO()
                {
                    UserId = userId,
                    NotificationId = notificationId
                };
                var result = await _userNotificationService.MarkNotificationAsRead(notification);

                if (!result.IsSuccess)
                {
                    return Response<bool>.Failure("Failed to mark notification as read", 500);
                }

                if (userConnections.TryGetValue(userId, out var connections) && connections.Any())
                {
                    await Task.WhenAll(connections.Select(connectionId =>
                        Clients.Client(connectionId).SendAsync("NotificationMarkedAsRead", notificationId)
                    ));
                }

                return Response<bool>.Success(true, "Notification marked as read successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"An error occurred while marking the notification as read{ex.Message}", 500);
            }
        }
        #endregion

    }
}
