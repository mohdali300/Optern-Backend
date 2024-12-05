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
        public string Content { get; set; }
        public DateTime SentDate { get; set; }

        //Foreign Keys
        public string SenderId { get; set; }
        public int ChatId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser Sender { get; set; }
        public virtual Chat Chat { get; set; }
    }
}
