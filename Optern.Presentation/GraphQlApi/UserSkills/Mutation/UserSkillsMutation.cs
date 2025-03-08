
namespace Optern.Presentation.GraphQlApi.UserSkills.Mutation
{
    [ExtendObjectType("Mutation")]
    public class UserSkillsMutation
    {
        [GraphQLDescription("Manage User Skills")]
        public async Task<Response<bool>> ManageUserSkillsAsync([Service] IUserSkillsService _userSkillsService, string userId, List<SkillInputDTO> skills)
            => await _userSkillsService.ManageUserSkillsAsync(userId, skills);

        [GraphQLDescription("Delete User Skills")]
        public async Task<Response<bool>> DeleteUserSkillsAsync([Service] IUserSkillsService _userSkillsService, string userId, List<int> skillsIds)
            => await _userSkillsService.DeleteUserSkillsAsync(userId, skillsIds);
    }
}
