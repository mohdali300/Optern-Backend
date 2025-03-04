using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.PTPInterview
{
    public class CreatePTPInterviewDTO
    {
        public CreatePTPInterviewDTO()
        {
            Category=InterviewCategory.Behavioral;
            ScheduledDate=string.Empty;
            ScheduledTime = InterviewTimeSlot.EightAM;
            QuestionType = InterviewQuestionType.Beginner;

        }
        public InterviewCategory Category { get; set; }
        public string ScheduledDate { get; set; }
        public InterviewTimeSlot ScheduledTime { get; set; }
        public InterviewQuestionType QuestionType { get; set; }
        
    }
}
