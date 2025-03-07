using Optern.Application.DTOs.Education;
using Optern.Application.Interfaces.IEducationService;

namespace Optern.Presentation.GraphQlApi.Education.Query
{
    [ExtendObjectType("Query")]
    public class EducationQuery
    {
        [GraphQLDescription("Get all user education")]
        public async Task<Response<IEnumerable<EducationDTO>>> GetUserEducationAsync([Service]IEducationService _educationService, string userId)
            => await _educationService.GetUserEducationAsync(userId);
    }
}
