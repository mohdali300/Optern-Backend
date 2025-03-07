using Optern.Application.DTOs.Education;
using Optern.Application.Interfaces.IEducationService;

namespace Optern.Presentation.GraphQlApi.Education.Mutation
{
    [ExtendObjectType("Mutation")]
    public class EducationMutation
    {
        [GraphQLDescription("Add education for user")]
        public async Task<Response<EducationDTO>> AddEducationAsync([Service] IEducationService _educationService, string userId, EducationInputDTO model)
            => await _educationService.AddEducationAsync(userId, model);

        [GraphQLDescription("Edit education")]
        public async Task<Response<EducationDTO>> EditEducationAsync([Service] IEducationService _educationService, string userId, int educatioId, EducationInputDTO model)
            => await _educationService.EditEducationAsync(userId, educatioId, model);

        [GraphQLDescription("Delete education")]
        public async Task<Response<bool>> DeleteEducationAsync([Service] IEducationService _educationService, string userId, int educatioId)
            => await _educationService.DeleteEducationAsync(userId,educatioId);
    }
}
