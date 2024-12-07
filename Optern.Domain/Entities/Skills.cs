using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class Skills
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }


        // Navigation Properties

        public virtual ICollection<UserSkills> UserSkills { get; set; }
        public virtual ICollection<RoomSkills> RoomSkills { get; set;}

    }
}
