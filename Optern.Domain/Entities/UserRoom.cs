using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class UserRoom
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string UserId { get; set; }

        // Navigation Property

        public virtual Room Room { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
