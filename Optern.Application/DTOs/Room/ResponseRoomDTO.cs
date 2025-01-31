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
    public class ResponseRoomDTO :BaseRoomDTO
    {
        public string? CreatorName { get; set; }
        public int? Members { get; set; } 
        public List<SkillsDTO>? Skills { get; set; } 
        public List<TrackDTO>? Tracks { get; set; }
        public List<SubTrackDTO>? SubTrack { get; set; }
        public ResponseRoomDTO()
        {
            Id = string.Empty;
            CreatorName = string.Empty;
            Members = 0;
            Skills = new List<SkillsDTO>();
            Tracks= new List<TrackDTO>();
            SubTrack= new List<SubTrackDTO>();
        }
    }
}
