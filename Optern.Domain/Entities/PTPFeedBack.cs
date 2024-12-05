using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class PTPFeedBack
    {
        public int Id { get; set; }

        public string InterviewerPerformance { get; set; }
        public string IntervieweePerformance { get; set; }

        // Foreign Keys
        public int PTPInterviewId { get; set; }
        // Navigation Properties
        public virtual PTPInterview PTPInterview { get; set; }

    }
}
