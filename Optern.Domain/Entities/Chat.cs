using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public DateTime CreatedDate  { get; set; }
        public ChatType Type { get; set; }

        //Foreign Keys
        public string CreatorId { get; set; }
        public string RoomId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser Creator { get; set; }
        public virtual ICollection<ChatParticipants> ChatParticipants { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

    }
}
