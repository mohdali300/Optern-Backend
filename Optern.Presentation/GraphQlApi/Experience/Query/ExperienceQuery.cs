using Optern.Application.DTOs.ExperienceDTO;
using Optern.Application.Interfaces.IExperienceService;

namespace Optern.Presentation.GraphQlApi.Experience.Query
{
    [ExtendObjectType("Query")]
    public class ExperienceQuery
    {
        public async Task<Response<IEnumerable<ExperienceDTO>>> GetUserExperiences(
            [Service] IExperienceService _experienceService, string userId) =>
            await _experienceService.GetUserExperiences(userId);
    }
}
