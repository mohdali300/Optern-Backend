using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.UserNotification
{
    public class UserNotificationDTO
    {
        public string UserId { get; set; }
        public int NotificationId { get; set; }

        public string Url { get; set; }=string.Empty;


    }
}
