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
        public int Answer {  get; set; }

        // Foreign Keys
        public int PTPInterviewId { get; set; }
        // Navigation Properties
        public virtual PTPInterview PTPInterview { get; set; }

    }
}
