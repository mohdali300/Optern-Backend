using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.PTPInterview
{
    public class PTPInterviewTimeSlotDTO
    {
        public PTPInterviewTimeSlotDTO()
        {
            TimeSlot= InterviewTimeSlot.EightAM;
            TimeSlotName=string.Empty;
            SlotState=TimeSlotState.Empty;
        }
        public InterviewTimeSlot TimeSlot { get; set; }          
        public TimeSlotState SlotState { get; set; }    
        public string TimeSlotName { get; set; }        
    }
}
