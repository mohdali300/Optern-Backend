using Optern.Application.DTOs.ExperienceDTO;
using Optern.Application.Interfaces.IExperienceService;
using Optern.Infrastructure.Services.Experience;

namespace Optern.Presentation.GraphQlApi.Experience.Mutation
{

    [ExtendObjectType("Mutation")]
    public class ExperienceMutation
    {
        public async Task<Response<bool>> AddExperience([Service] IExperienceService _experienceService, string userId,
            ExperienceDTO model) =>
            await _experienceService.AddExperience(userId, model);

        public async Task<Response<bool>> EditExperience([Service] IExperienceService _experienceService, int id,
            ExperienceDTO model) =>
            await _experienceService.EditExperience(id, model);

        public async Task<Response<bool>> DeleteExperience([Service] IExperienceService _experienceService, int id) =>
            await _experienceService.DeleteExperience(id);
    }
}
