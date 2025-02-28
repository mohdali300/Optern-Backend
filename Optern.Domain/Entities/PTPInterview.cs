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
		public InterviewCategory Category { get; set; }
        public InterviewQuestionType QusestionType { get; set; }
        public string? ScheduledDate { get; set; }
        public InterviewTimeSlot ScheduledTime { get; set; }
        public TimeSlotState SlotState { get; set; }

        public InterviewStatus Status { get; set; }
        // Navigation Properties
        public virtual ICollection<PTPUsers> PeerToPeerInterviewUsers { get; set; }
        public ICollection<PTPQuestionInterview> PTPQuestionInterviews { get; set; }
        public virtual ICollection<PTPFeedBack> PTPFeedBacks { get; set; }

	}
}
