
namespace Optern.Presentation.GraphQlApi.VFeedback.Mutation
{
    [ExtendObjectType("Mutation")]
    public class VFeedbackMutation
    {
        [GraphQLDescription("Add Vitual FeedBack for The Interview")]
        public async Task<Response<string>> AddVFeedback ([Service] IVFeedbackService _VFeedbackService, VFeedbackDTO model)
       => await _VFeedbackService.AddVFeedback(model);
    }
}
