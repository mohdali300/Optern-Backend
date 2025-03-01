namespace Optern.Presentation.GraphQlApi.PTPInterview.Mutation
{
    [ExtendObjectType("Mutation")]
    public class PTPInterviewMutation
    {
        [GraphQLDescription("Create PTP Interview")]

        public async Task<Response<PTPInterviewDTO>> CreatePTPInterviewAsync([Service] IPTPInterviewService _pTPInterviewService,CreatePTPInterviewDTO dto, int questionCount, string userId)
         => await _pTPInterviewService.CreatePTPInterviewAsync(dto,questionCount,userId);
    }
}
