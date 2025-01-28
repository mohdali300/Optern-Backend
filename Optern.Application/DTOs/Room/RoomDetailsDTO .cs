using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Room
{
    public class RoomDetailsDTO :BaseRoomDTO
    {
        public string Id { get; set; }
        public string CreatorName { get; set; }
        public int Members { get; set; } 
        public List<string> Skills { get; set; } 
        public RoomDetailsDTO()
        {
            Id = string.Empty;
            CreatorName = string.Empty;
            Members = 0;
            Skills = new List<string>();
        }
    }
}
