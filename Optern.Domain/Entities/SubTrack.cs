using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
	public class SubTrack
	{
		public int Id { get; set; }
		public string Name { get; set; }

        // Foreign Keys
		public int TrackId { get; set; }

		// Navigation Properties
		public virtual Track Track { get; set; }
	}
}
