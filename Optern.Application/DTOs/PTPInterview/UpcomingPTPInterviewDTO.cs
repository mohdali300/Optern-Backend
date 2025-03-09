using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.PTPInterview
{
    public class UpcomingPTPInterviewDTO
    {
        public int Id { get; set; }
        public string? ScheduledDate { get; set; } 
        public string? ScheduledTime { get; set; }
        public string? Category { get; set; }
        public List<PTPUpcomingQuestionDTO>? Questions { get; set; }
      //  public string? TimeRemaining { get; set; }

        public UpcomingPTPInterviewDTO()
        {
            Questions = new List<PTPUpcomingQuestionDTO>();
        }
    }

}
