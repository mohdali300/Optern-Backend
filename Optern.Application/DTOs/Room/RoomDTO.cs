using Microsoft.AspNetCore.Http;
using Optern.Application.DTOs.Skills;
using Optern.Application.DTOs.SubTrack;
using Optern.Application.DTOs.Track;
using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Room
{
    public class RoomDTO
    {
        public RoomDTO()
        {
            Name = string.Empty;
            Description = string.Empty;
            Capacity = 0;
            NumberOfParticipants = 0;
            RoomType = 0;
            CoverPicture = string.Empty;
            CreatedAt = DateTime.Now;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public int? NumberOfParticipants { get; set; }
        public RoomType RoomType { get; set; }
        public string? CoverPicture { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatorId { get; set; }
        public ICollection<int>? SubTracks { get; set; } = new HashSet<int>();
        public ICollection<int>? Skills { get; set; } = new List<int>();
    }
}
