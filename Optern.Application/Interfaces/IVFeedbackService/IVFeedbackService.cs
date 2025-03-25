

using Optern.Application.DTOs.VFeedback;

namespace Optern.Application.Interfaces.IVFeedbackService
{
    public interface IVFeedbackService
    {
        public  Task<Response<VFeedBack>> AddVFeedback(VFeedbackDTO model);
        public  Task<Response<VFeedbackDTO>> GetVFeedback(int vInterviewId);


    }
}
