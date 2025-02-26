

namespace Optern.Application.Interfaces.IPTPInterviewService
{
    public interface IPTPInterviewService
    {
        public Task<Response<IEnumerable<UpcomingPTPInterviewDTO>>> GetAllUpcomingPTPInterviews(string userId);

    }
}
