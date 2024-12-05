using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
	public class Experience
	{
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsCurrentlyWork { get; set; }
        public string JobDescription { get; set; }
        public string Location { get; set; }

        // Foreign Key
        public string UserId { get; set; }

		// Navigation Properties
		public virtual ApplicationUser User { get; set; }
    }
}
