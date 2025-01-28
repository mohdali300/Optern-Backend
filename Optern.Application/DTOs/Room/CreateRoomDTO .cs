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
    public class CreateRoomDTO:BaseRoomDTO
    {
        public CreateRoomDTO()
        {
            Capacity = 0;
            NumberOfParticipants = 0;
            CreatorId = string.Empty;
            SubTracks= new HashSet<int>();
            Skills = new List<int>();
        }
        public int Capacity { get; set; } 
        public int? NumberOfParticipants { get; set; }
        public string CreatorId { get; set; } 
        public ICollection<int>? SubTracks { get; set; }
        public ICollection<int>? Skills { get; set; } 
    }
}
