using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Infrastructure.ExternalDTOs.UserNotification;

namespace Optern.Infrastructure.ExternalInterfaces.IUserNotificationService
{
    public interface IUserNotificationService
    {
        public Task<Response<string>> SaveNotification(UserNotificationDTO model);
        public Task<Response<bool>> DeleteUserNotification(UserNotificationDTO model);
    }
}
