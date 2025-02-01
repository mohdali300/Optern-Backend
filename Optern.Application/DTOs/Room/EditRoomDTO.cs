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
    public class EditRoomDTO 
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
        public RoomType? RoomType { get; set; }
        public int? Capacity { get; set; }
        public List<int>? SubTracks { get; set; }=new List<int>(); 
        public List<SkillsDTO>? Skills { get; set; }=new List<SkillsDTO>();

    }
}
