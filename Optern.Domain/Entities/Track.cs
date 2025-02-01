using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
	public class Track
	{
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation Properties
        public  virtual ICollection<RoomTrack> RoomTracks { get; set; }
    }
}
