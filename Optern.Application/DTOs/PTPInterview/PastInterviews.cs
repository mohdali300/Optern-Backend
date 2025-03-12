using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.PTPInterview
{
    public class PastInterviews
    {
        public int Id { get; set; }
        [JsonIgnore] 
        public DateTime? InterviewDate { get; set; }

        public string InterviewDateFormatted => InterviewDate?.ToString("dd-MM-yyyy") ?? "N/A";
        public string? InterviewType { get; set; } 
        public string? Category { get; set; }
        public PartnerDTO Partner { get; set; } = new PartnerDTO(); 

        public List<PTPUpcomingQuestionDTO>? Questions { get; set; }

      
    }


}
