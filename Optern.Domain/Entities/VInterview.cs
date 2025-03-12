using Optern.Domain.Enums;
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
		public InterviewCategory Category { get; set; }
        public InterviewQuestionType QusestionType { get; set; }
        public string? SpeechAnalysisResult { get; set; }

        public DateTime InterviewDate { get; set; }= DateTime.UtcNow;


        //Foreign Keys
        public string UserId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; }
        public virtual VFeedBack VirtualFeedBack { get; set; }
        public ICollection<PTPQuestionInterview> VQuestionInterviews { get; set; }

    }
}
