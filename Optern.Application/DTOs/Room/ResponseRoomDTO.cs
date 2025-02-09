

namespace Optern.Application.DTOs.Room
{
	public class ResponseRoomDTO :BaseRoomDTO
	{
		public string? CreatorId{ get; set; }
		public int? Members { get; set; } 
		public List<SkillDTO>? Skills { get; set; } 
		public List<TrackDTO>? Tracks { get; set; }
		public List<PositionDTO>? Position { get; set; }
		public UserRoomStatus UserStatus { get; set; }
		public ResponseRoomDTO()
		{
			Id = string.Empty;
			CreatorId = string.Empty;
			Members = 0;
			Skills = new List<SkillDTO>();
			Tracks= new List<TrackDTO>();
			Position= new List<PositionDTO>();
		}
	}
}
