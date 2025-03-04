namespace Optern.Presentation.GraphQlApi.PTPFeedback.Query
{
    [ExtendObjectType("Query")]
    public class PTPFeedbackQuery
    {
        [GraphQLDescription("Get PTP Feedback")]
        public async Task<Response<PTPFeedbackDTO>> GetPTPFeedback([Service] IPTPFeedbackService PTPFeedbackService, int ptpInterviewId, string userId)
            => await PTPFeedbackService.GetPTPFeedback(ptpInterviewId, userId);


    }
}
