namespace Optern.Application.Interfaces.IPTPFeedbackService
{
    public interface IPTPFeedbackService
    {
        public Task<Response<string>> AddPTPFeedback(AddPTPFeedbackDTO model);
        public Task<Response<PTPFeedbackDTO>> GetPTPFeedback(int ptpInterviewId, string userId);

    }
}
