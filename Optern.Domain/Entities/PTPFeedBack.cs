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
        // in scale from 1 to 5
        public Dictionary<int, string> Ratings { get; set; } = new Dictionary<int, string>();
        public string InterviewerPerformance {  get; set; }
        public string Improvement {  get; set; }

        public int GivenByUserId { get; set; }
        public  int ReceivedByUserId { get; set; }
        public int PTPInterviewId { get; set; }
        // Navigation Properties
        public virtual PTPInterview PTPInterview { get; set; }
        public virtual PTPUsers ReceivedByUser { get; set; }
        public virtual PTPUsers GivenByUser { get; set; }

    }
}
