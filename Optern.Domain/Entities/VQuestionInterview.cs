using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class VQuestionInterview
    {
        public int Id { get; set; }

        // Foreign Keys
        public int PTPQuestionId { get; set; }
        public int VInterviewId { get; set; }
        public string UserId { get; set; }


        // Navigation Properties

        public virtual PTPQuestions PTPQuestion { get; set; }
        public virtual VInterview VInterview { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
