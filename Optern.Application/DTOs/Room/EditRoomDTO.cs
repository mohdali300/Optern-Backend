
namespace Optern.Application.DTOs.Room
{
    public class EditRoomDTO 
    {

        public string? Name { get; set; }
        public string? Description { get; set; }
        public RoomType? RoomType { get; set; }
        public List<int>? Positions { get; set; }=new List<int>(); 
        public List<int>? Tracks { get; set; }=new List<int>(); 
        public List<SkillDTO>? Skills { get; set; }=new List<SkillDTO>();

    }
}
