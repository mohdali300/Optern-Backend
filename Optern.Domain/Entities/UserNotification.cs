using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class UserNotification
    {
        public int Id { get; set; }

        // Foreign Keys
        public string  UserId { get; set; } 
        public int NotificationId { get; set; }

        public bool IsRead { get; set; } = false;

        // Navigation Properties
        public virtual ApplicationUser User { get; set; }
        public virtual Notifications Notifications { get; set; }

    }
}
