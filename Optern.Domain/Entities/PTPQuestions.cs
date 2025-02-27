using System;
using System.Collections.Generic;
using System.Linq;

using Optern.Domain.Enums;

namespace Optern.Domain.Entities
{
    public class PTPQuestions
    {
        public int Id {  get; set; }
        public  InterviewQuestionType QusestionType { get; set; } 

        public string Title { get; set; }

        public string Content {  get; set; }

        public string? Hints { get; set; }

        public string Answer {  get; set; }

        // Navigation Properties
        public ICollection<PTPQuestionInterview> PTPQuestionInterviews { get; set; } 
    }
}
