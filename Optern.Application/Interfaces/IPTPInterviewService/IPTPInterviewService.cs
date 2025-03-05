

using Optern.Application.DTOs.Question;

namespace Optern.Application.Interfaces.IPTPInterviewService
{
    public interface IPTPInterviewService
    {
        public Task<Response<IEnumerable<UpcomingPTPInterviewDTO>>> GetAllUpcomingPTPInterviews(string userId);
        public Task<Response<PTPInterviewDTO>> CreatePTPInterviewAsync(CreatePTPInterviewDTO dto, int questionCount, string userId);
        public Task<Response<List<PTPInterviewTimeSlotDTO>>> GetPTPInterviewTimeSlotsAsync(InterviewCategory category, InterviewQuestionType questionType, string scheduledDate);
        public  Task<Response<bool>> CancelPTPInterviewAsync(int interviewId, string userId);



    }
}
