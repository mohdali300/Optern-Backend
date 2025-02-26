using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
   public  class PTPQuestionInterview
    {
          public int Id { get; set; }

        // Foreign Keys
        public int PTPQuestionId { get; set; }
        public int PTPInterviewId { get; set; }

        // Navigation Properties

        public virtual PTPQuestions PTPQuestion { get; set; }
        public virtual PTPInterview PTPInterview { get; set; }

    }
}
