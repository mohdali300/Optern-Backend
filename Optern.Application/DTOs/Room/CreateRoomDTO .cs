

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
