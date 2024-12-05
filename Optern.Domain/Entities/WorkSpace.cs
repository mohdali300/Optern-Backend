using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class WorkSpace
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }

        // Foreign Keys
        public int RoomId { get; set; }

        // Navigation Properties
        public virtual Room Room { get; set; }
        public virtual ICollection<Sprint> Sprints { get; set; }
    }
}
