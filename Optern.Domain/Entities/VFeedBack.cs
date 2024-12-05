using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class VFeedBack
    {
        public int Id { get; set; }
        public int PerformanceScore { get; set; }
        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public string Recommendations { get; set; }

        // Foreign Keys
        public int VInterviewID { get; set; }

        // Navigation Properties
        public virtual VInterview VirtualInterview { get; set; }

    }
}
