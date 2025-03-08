using Optern.Application.DTOs.ExperienceDTO;
using Optern.Application.Interfaces.IExperienceService;

namespace Optern.Presentation.GraphQlApi.Experience.Mutation
{

    [ExtendObjectType("Mutation")]
    public class ExperienceMutation
    {
        public async Task<Response<bool>> AddExperience([Service] IExperienceService _experienceService, string userId,
            ExperienceDTO model) =>
            await _experienceService.AddExperience(userId, model);
    }
}
