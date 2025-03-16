using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.VFeedback
{
    public class VFeedbackDTO
    {
        public int? Id { get; set; }
        public int PerformanceScore { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public string? Recommendations { get; set; }
        public int VirtualInterviewId { get; set; }

        public VFeedbackDTO()
        {
            Id = 0;
            PerformanceScore = 0;
            Strengths = "N/A";
            Weaknesses = "N/A";
            Recommendations = "N/A";
            VirtualInterviewId = 0;
        }
    }

}
