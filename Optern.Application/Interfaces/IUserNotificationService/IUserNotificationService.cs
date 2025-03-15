

namespace Optern.Application.Interfaces.IUserNotificationService
{
    public interface IUserNotificationService
    {
        public Task<Response<string>> SaveNotification(UserNotificationDTO model);
        public Task<Response<bool>> DeleteUserNotification(UserNotificationDTO model);
        public Task<Response<string>> MarkNotificationAsRead(UserNotificationDTO model);
        public Task<Response<IEnumerable<GetUserNotificationDTO>>> GetUserNotifications(string userId, bool? isRead = null);
        public Task<Response<IEnumerable<GetUserNotificationDTO>>> SearchUserNotifications(SearchUserNotificationsDTO model, int lastIdx = 0, int limit = 10);

    }
}
