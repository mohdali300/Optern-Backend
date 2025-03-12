using Optern.Application.DTOs.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.VInterview
{
    public class VInterviewDTO
    {
        public VInterviewDTO()
        {
            Id = 0;
            Category = InterviewCategory.Behavioral;
            QusestionType = InterviewQuestionType.Beginner;
            UserId=string.Empty;    
            Questions = new();
        }
        public int Id { get; set; }
        public InterviewCategory Category { get; set; }
        public InterviewQuestionType QusestionType { get; set; }
        public string UserId { get; set; }
        public List<PTPQuestionDTO> Questions { get; set; } = new();
    }
}
