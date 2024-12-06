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

        // Navigation Properties
        public ICollection<VInterviewQuestions> VInterviewQuestions { get; set; }

    }
}
