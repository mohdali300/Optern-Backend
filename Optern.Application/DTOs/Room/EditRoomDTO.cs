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
    public class EditRoomDTO 
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
        public RoomType? RoomType { get; set; }
        public int? Capacity { get; set; }
        public List<int>? Positions { get; set; }=new List<int>(); 
        public List<int>? Tracks { get; set; }=new List<int>(); 
        public List<SkillDTO>? Skills { get; set; }=new List<SkillDTO>();

    }
}
