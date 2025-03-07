
using Optern.Application.DTOs.Education;

namespace Optern.Application.Interfaces.IEducationService
{
    public interface IEducationService
    {
        public Task<Response<EducationDTO>> AddEducationAsync(string userId, EducationInputDTO model);
        public Task<Response<EducationDTO>> EditEducationAsync(string userId,int educatioId, EducationInputDTO model);
        public Task<Response<bool>> DeleteEducationAsync(string userId,int educatioId);
        public Task<Response<IEnumerable<EducationDTO>>> GetUserEducationAsync(string userId);
    }
}
