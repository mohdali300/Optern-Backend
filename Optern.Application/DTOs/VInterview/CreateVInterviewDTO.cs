using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.VInterview
{
    public class CreateVInterviewDTO
    {
        public CreateVInterviewDTO()
        {
            Category = InterviewCategory.Behavioral;
            QuestionType = InterviewQuestionType.Beginner;

        }
        public InterviewCategory Category { get; set; }
        public InterviewQuestionType QuestionType { get; set; }
    }
}
