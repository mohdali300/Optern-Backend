using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class ChatParticipants
    {
        public int  Id  { get; set; }
        public DateTime JoinedAt { get; set; }
        // Foreign Keys

        public int ChatId { get; set; }
        public string UserId { get; set; }

        // Navigation Properties
        public virtual Chat Chat { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
