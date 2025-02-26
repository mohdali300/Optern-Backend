using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.PTPFeedback
{
    public class PTPFeedbackDTO
    {
        public int? Id { get; set; }
        public List<KeyValuePair<int, string>> Ratings { get; set; } = new List<KeyValuePair<int, string>>();
        public string InterviewerPerformance { get; set; } = string.Empty;
        public string Improvement { get; set; } = string.Empty;
        public int? GivenByUserId { get; set; }
        public int? ReceivedByUserId { get; set; } 
        public int? PTPInterviewId { get; set; }
        public PTPFeedbackDTO() { }
    }


}
