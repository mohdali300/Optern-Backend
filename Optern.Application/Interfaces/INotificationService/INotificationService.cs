

namespace Optern.Application.Interfaces.INotificationService
{
    public interface INotificationService
    {
        public Task<Response<NotificationResponseDTO>> AddNotification(NotificationDTO model);
        public Task<bool> IsNotificationExist(int notificationId);
    }
}
