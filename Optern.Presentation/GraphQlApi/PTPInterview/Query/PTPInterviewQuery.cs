
using Optern.Application.DTOs.Question;

namespace Optern.Presentation.GraphQlApi.PTPInterview.Query
{
    [ExtendObjectType("Query")]
    public class PTPInterviewQuery
    {
        [GraphQLDescription("Get Upcoming PTP Interview")]

        public async Task<Response<IEnumerable<UpcomingPTPInterviewDTO>>> GetAllUpcomingPTPInterviews([Service] IPTPInterviewService PTPInterviewService
            , string userId) => await PTPInterviewService.GetAllUpcomingPTPInterviews(userId);


    }
}
