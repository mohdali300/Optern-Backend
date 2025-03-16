
namespace Optern.Presentation.GraphQlApi.VFeedback.Query
{
    [ExtendObjectType("Query")]
    public class VFeedBackQuery
    {
        [GraphQLDescription("Get Virtual Interview")]
        public async Task<Response<VFeedbackDTO>> GetVFeedback([Service] IVFeedbackService _VFeedbackService, int vInterviewId)
            => await _VFeedbackService.GetVFeedback(vInterviewId);

    }
}
