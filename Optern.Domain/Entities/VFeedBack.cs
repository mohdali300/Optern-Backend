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
        public string? Intro { get; set; }
        public string? Recommendations { get; set; }

        // Foreign Key
        public int VirtualInterviewId { get; set; }

        // Navigation Property
        public virtual VInterview VirtualInterview { get; set; }
    }

}
