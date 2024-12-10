using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Domain.Enums;

namespace Optern.Domain.Entities
{
    public class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public RoomType RoomType { get; set; }

        public string CoverPicture { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys

        public string CreatorId { get; set; }

		// Navigation Property
		public virtual ApplicationUser Creator { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual ICollection<SubTrack> SubTracks { get; set; }
		public virtual ICollection<UserRoom> UserRooms { get; set; }
        public virtual ICollection<Notes> Notes { get; set; }
        public virtual ICollection<Notifications> Notifications { get; set; }
        public virtual ICollection<WorkSpace> WorkSpaces { get; set; }

        public virtual ICollection<RoomSkills> RoomSkills { get; set;}
        public virtual ICollection<RoomTrack> RoomTracks { get; set; }


    }
}
