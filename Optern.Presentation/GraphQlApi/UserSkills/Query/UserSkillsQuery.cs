using Optern.Application.DTOs.Skills;
using Optern.Application.Interfaces.IUserSkillsService;

namespace Optern.Presentation.GraphQlApi.UserSkills.Query
{
    [ExtendObjectType("Query")]
    public class UserSkillsQuery
    {
        [GraphQLDescription("Get all user skills")]
        public async Task<Response<IEnumerable<SkillDTO>>> GetUserSkillsAsync([Service] IUserSkillsService _userSkillsService, string userId)
            => await _userSkillsService.GetUserSkillsAsync(userId);
    }
}
