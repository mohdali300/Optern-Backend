using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Room.RoomDTO
{
    public class RoomDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Capacity { get; set; }
        public RoomType RoomType { get; set; }
        public string CoverPicture { get; set; }
        public List<string> Tracks { get; set; } = new List<string>();
        public List<string> SubTracks { get; set; } = new List<string>();
        public List<string> Skills { get; set; } = new List<string>();
    }
}
