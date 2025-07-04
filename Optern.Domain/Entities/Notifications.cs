﻿    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class Notifications
    {
        public int Id  { get; set; }
        public string Message  { get; set; }
        public string? Title { get; set; }

        // Navigation Properties
        public virtual ICollection<UserNotification> UserNotification { get; set; }

    }
}
