using Optern.Application.DTOs.Question;

namespace Optern.Application.Interfaces.IPTPInterviewService
{
    public interface IPTPInterviewService
    {
        public Task<Response<IEnumerable<UpcomingPTPInterviewDTO>>> GetAllUpcomingPTPInterviews(string userId);
        public Task<Response<PTPInterviewDTO>> CreatePTPInterviewAsync(CreatePTPInterviewDTO dto, int questionCount, string userId);
        public Task<Response<List<PTPInterviewTimeSlotDTO>>> GetPTPInterviewTimeSlotsAsync(InterviewCategory category, InterviewQuestionType questionType, string scheduledDate);
        public  Task<Response<bool>> CancelPTPInterviewAsync(int interviewId, string userId);
        public Task<Response<List<PTPQuestionDTO>>> GetUserPTPInterviewQuestionsAsync(int interviewId, string userId);
        public Task<Response<bool>> StartPTPInterviewSessionAsync(int interviewId, string userId);
        public Task<Response<bool>> EndPTPInterviewSessionAsync(int interviewId);
        public Task<Response<PTPInterviewDTO>> GetUserCurrentPTPInterviewSessionAsync(string userId);
        public System.Threading.Tasks.Task HandleInterviewStatus();
        public Task<Response<IEnumerable<PastInterviews>>> PastInterviews(string userId);
        public Task<Response<PTPInterview>> GetInterviewTimeSlot(int interviewId);
        public TimeSpan GetTimeSpanFromEnum(InterviewTimeSlot timeSlot);
        public Response<InterviewCachedData> LoadInterviewCachedData(int interviewId);

    }
}
