using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
   public class RoomSkillsDTO
    {
         public int Id { get; set; }

        // Foreign Keys
        public string RoomId { get; set; }
        public int SkillId { get; set; }

        // Navigation Properties
        public virtual Room Room { get; set; }
        public virtual Skills Skill { get; set; }
    }
}
