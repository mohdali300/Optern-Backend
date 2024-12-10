using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class PTPUsers
    {
        public int Id { get; set; } 
        public int PTPIId { get; set; }
        public string UserID { get; set; }

        // Navigation Properties

        public virtual ApplicationUser User { get; set; }
        public virtual PTPInterview PeerToPeerInterview { get; set; }

    }
}
