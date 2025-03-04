using Optern.Application.DTOs.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.PTPInterview
{
    public class PTPInterviewDTO
    {
        public PTPInterviewDTO()
        {
            Id = 0;
            Category = InterviewCategory.Behavioral;
            ScheduledDate = string.Empty;
            ScheduledTimeDisplay=string.Empty;
            SlotState = TimeSlotState.Empty;
            Status = InterviewStatus.Scheduled;
            Questions = new();
        }
        public int Id { get; set; }
        public InterviewCategory Category { get; set; }
        public string ScheduledDate { get; set; }
        public string? ScheduledTimeDisplay { get; set; }
        public TimeSlotState SlotState { get; set; }
        public InterviewStatus Status { get; set; }
        public List<PTPQuestionDTO> Questions { get; set; } = new();
    }
}
