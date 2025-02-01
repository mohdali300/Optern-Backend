using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
	public class RoomPosition
	{
		public int Id { get; set; }

		// Foreign Keys
		public int PositionId { get; set; }
		public string RoomId { get; set; }

		// Navigation Properties
		public virtual Position Position { get; set; }
		public virtual Room Room { get; set; } 
	}
}
