using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.RoomUset
{
    public class RoomUserDTO
    {
        public RoomUserDTO()
        {
             UserId =string.Empty;
             UserName =string.Empty;
             ProfilePicture = string.Empty;
             IsAdmin = false;
             JoinedAt = DateTime.UtcNow;
    }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsAdmin { get; set; }=false;
        public DateTime? JoinedAt { get; set; }

        public string? Role => IsAdmin ? "Leader" : "Collaborator";
    }
}
