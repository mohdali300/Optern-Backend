namespace Optern.Presentation.GraphQlApi.PTPInterview.Mutation
{
    [ExtendObjectType("Mutation")]
    public class PTPInterviewMutation
    {
        [GraphQLDescription("Create PTP Interview")]
        public async Task<Response<PTPInterviewDTO>> CreatePTPInterviewAsync([Service] IPTPInterviewService _pTPInterviewService,CreatePTPInterviewDTO dto, int questionCount, string userId)
         => await _pTPInterviewService.CreatePTPInterviewAsync(dto,questionCount,userId);


        [GraphQLDescription("Cancel PTP Interview")]
        public async Task<Response<bool>> CancelPTPInterviewAsync([Service] IPTPInterviewService _pTPInterviewService, int interviewId, string userId)
            => await _pTPInterviewService.CancelPTPInterviewAsync(interviewId, userId);

        [GraphQLDescription("Start PTP Interview Session")]
        public async Task<Response<bool>> StartPTPInterviewSessionAsync([Service] IPTPInterviewService _pTPInterviewService, int interviewId, string userId)
            => await _pTPInterviewService.StartPTPInterviewSessionAsync(interviewId, userId);

        [GraphQLDescription("End PTP Interview Session")]
        public async Task<Response<bool>> EndPTPInterviewSessionAsync([Service] IPTPInterviewService _pTPInterviewService, int interviewId)
            => await _pTPInterviewService.EndPTPInterviewSessionAsync(interviewId);

        
    }
}
