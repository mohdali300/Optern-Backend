using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class VInterview
    {
        public int Id { get; set; }
        //public List<string> GeneratedQuestions { get; set; }
        public string SpeechAnalysisResult { get; set; }
        public DateTime ScheduledTime { get; set; }


        //Foreign Keys
        public string UserId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<VQuestions> VirtualQuestions { get; set; }
    }
}
