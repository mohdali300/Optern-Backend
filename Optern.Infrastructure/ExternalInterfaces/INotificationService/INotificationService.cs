using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Infrastructure.ExternalDTOs.Notification;

namespace Optern.Infrastructure.ExternalInterfaces.INotificationService
{
    public interface INotificationService
    {
        public Task<Response<bool>> AddNotification(NotificationDTO model);
        public Task<bool> IsNotificationExist(int notificationId);
    }
}
