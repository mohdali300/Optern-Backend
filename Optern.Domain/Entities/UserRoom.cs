using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class UserRoom
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string RoomId { get; set; }
        public string UserId { get; set; }
       
        public DateTime JoinedAt { get; set; }=DateTime.Now;
        
        public bool IsAdmin { get; set; } 
        public bool IsAccepted { get; set; }


        // Navigation Property

        public virtual Room Room { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
