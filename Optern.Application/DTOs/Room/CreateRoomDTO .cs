using Microsoft.AspNetCore.Http;
using Optern.Application.DTOs.Skills;
using Optern.Application.DTOs.Position;
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
            NumberOfParticipants = 0;
            CreatorId = string.Empty;
            Positions= new HashSet<int>();
            Skills = new List<SkillDTO>();
            Tracks= new List<int>();
        }
        public int? NumberOfParticipants { get; set; }
        public string CreatorId { get; set; } 
        public ICollection<int>? Positions { get; set; }
        public ICollection<int>? Tracks { get; set; }
        public ICollection<SkillDTO>? Skills { get; set; } 
    }
}
