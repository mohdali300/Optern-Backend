

namespace Optern.Application.DTOs.Room
{
	public class BaseRoomDTO
	{
		public string? Id { get; set; }
		public string Name { get; set; } 
		public string? Description { get; set; }
		public RoomType RoomType { get; set; } 
		public string? CoverPicture { get; set; } 
		public DateTime? CreatedAt { get; set; }


        public BaseRoomDTO()
		{
			Name= string.Empty;
			Description= string.Empty;
			RoomType=Domain.Enums.RoomType.Private;
			CoverPicture= string.Empty;
			CreatedAt= DateTime.MinValue;

		}
	}
}
