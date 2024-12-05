using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class PTPInterview
    {
        public int Id { get; set; }
        public DateTime ScheduledTime { get; set; }
        public InterviewStatus Status { get; set; }
        public TimeSpan Duration { get; set; }

        // Navigation Properties
        public virtual ICollection<PTPUsers> PeerToPeerInterviewUsers { get; set; }
        public virtual ICollection<PTPQuestions> PeerToPeerQuestions { get; set; }



    }
}
