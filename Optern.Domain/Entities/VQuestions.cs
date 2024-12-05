using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class VQuestions
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Answer { get; set; }
        
        // Foreign Keys
        public int VInterviewID { get; set; }


        // Navigation Properties
    public virtual VInterview VInterview { get; set; }
    }
}
