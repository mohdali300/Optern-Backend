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
        public string? Intro { get; set; }
        public string? Recommendations { get; set; }
        public int VirtualInterviewId { get; set; }

        public VFeedbackDTO()
        {
            Id = 0;
            Intro = "N/A";
            Recommendations = "N/A";
            VirtualInterviewId = 0;
        }
    }

}
