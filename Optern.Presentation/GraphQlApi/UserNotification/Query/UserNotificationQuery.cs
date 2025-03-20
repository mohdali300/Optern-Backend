
namespace Optern.Presentation.GraphQlApi.UserNotification.Query
{
    [ExtendObjectType("Query")]
    public class UserNotificationQuery
    {
        [GraphQLDescription("Get Notifications For User")]
        public async Task<Response<IEnumerable<GetUserNotificationDTO>>> GetUserNotifications([Service] IUserNotificationService _notificationService,
             string userId, string roomId, bool? isRead = null , int lastIdx = 0, int limit = 10) =>
             await _notificationService.GetUserNotifications(userId, roomId, isRead,lastIdx,limit);

        [GraphQLDescription("Search Notifications For User")]
        public async Task<Response<IEnumerable<GetUserNotificationDTO>>> SearchUserNotifications([Service] IUserNotificationService _notificationService, 
            SearchUserNotificationsDTO model, int lastIdx = 0, int limit = 10)=>
            await _notificationService.SearchUserNotifications(model,lastIdx,limit);



    }
}
