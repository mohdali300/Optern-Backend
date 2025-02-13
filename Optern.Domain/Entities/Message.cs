using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime SentAt { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsRead { get; set; } = false;


        //Foreign Keys
        public string SenderId { get; set; }
        public int ChatId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser Sender { get; set; }
        public virtual Chat Chat { get; set; }
    }
}
