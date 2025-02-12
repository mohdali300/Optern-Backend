using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalDTOs.Notification
{
    public class NotificationDTO
    {

        public string Title { get; set; }
        public string Message { get; set; }
        public string? RoomId { get; set; }
    }
}