

namespace Optern.Application.Interfaces.IUserNotificationService
{
    public interface IUserNotificationService
    {
        public Task<Response<string>> SaveNotification(UserNotificationDTO model);
        public Task<Response<bool>> DeleteUserNotification(UserNotificationDTO model);
    }
}
