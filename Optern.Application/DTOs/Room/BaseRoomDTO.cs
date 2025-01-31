using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Room
{
	public class BaseRoomDTO
	{
		public string? Id { get; set; }
		public string Name { get; set; } 
		public string Description { get; set; }
		public RoomType RoomType { get; set; } 
		public string? CoverPicture { get; set; } 
		public DateTime? CreatedAt { get; set; }


        public BaseRoomDTO()
		{
			Name= string.Empty;
			Description= string.Empty;
			RoomType= RoomType.Public;
			CoverPicture= string.Empty;
			CreatedAt= DateTime.MinValue;

		}
	}
}
