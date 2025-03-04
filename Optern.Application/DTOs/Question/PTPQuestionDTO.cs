using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Question
{
    public class PTPQuestionDTO
    {
        public PTPQuestionDTO()
        {
            Id = 0;
            QusestionType = InterviewQuestionType.Beginner;
            Category = InterviewCategory.SQL;
            Title=string.Empty;
            Content = string.Empty; 
            Hints = string.Empty;   
            Answer=string.Empty;
        }
        public int Id { get; set; }
        public InterviewQuestionType? QusestionType { get; set; }
        public InterviewCategory? Category { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Hints { get; set; }
        public string? Answer { get; set; }
    }
}
