using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class VInterviewQuestions
    {
        public int Id { get; set; }

        public int VInterviewID { get; set; }
        public int VQuestionID { get; set; }

        // Navigation Properties
        public virtual VInterview VInterview { get; set; }
        public virtual VQuestions VQuestions  { get; set; }

    }
}
