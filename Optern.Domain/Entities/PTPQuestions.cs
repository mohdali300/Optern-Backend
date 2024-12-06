using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class PTPQuestions
    {
        public int Id {  get; set; }
        public string Content {  get; set; }
        public string Answer {  get; set; }

        // Navigation Properties
        public ICollection<PTPQuestionInterview> PTPQuestionInterviews { get; set; } 
    }
}
