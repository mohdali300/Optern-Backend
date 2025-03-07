using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
	public class Education
	{
        public int Id { get; set; }
        public string School { get; set; }
        public string University { get; set; }
		public EducationDegree Degree { get; set; } = EducationDegree.Bachelor;
		public string Major { get; set; }
		public string StartYear { get; set; } = DateTime.UtcNow.Year.ToString();
		public string EndYear { get; set; } = DateTime.UtcNow.Year.ToString();

        // Foreign Key
        public string UserId { get; set; }

		// Navigation Properties
		public virtual ApplicationUser User { get; set; }

	}
}
