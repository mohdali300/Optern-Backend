
namespace Optern.Presentation.GraphQlApi.PTPFeedback.Mutation
{
    [ExtendObjectType("Mutation")]

    public class PTPFeedbackMutation
    {

        [GraphQLDescription("Create a PTP Feedback")]
        public async Task<Response<string>> AddPTPFeedbackAsync([Service] IPTPFeedbackService PTPFeedbackService,
            AddPTPFeedbackDTO model)
            => await PTPFeedbackService.AddPTPFeedback(model);
    }
}
