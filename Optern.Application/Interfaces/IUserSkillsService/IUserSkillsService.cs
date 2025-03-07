
namespace Optern.Application.Interfaces.IUserSkillsService
{
    public interface IUserSkillsService
    {
        public Task<Response<bool>> AddUserSkillsAsync(string userId, List<int> skills);
        public Task<Response<bool>> ManageUserSkillsAsync(string userId,List<SkillInputDTO> skills);
        public Task<Response<bool>> DeleteUserSkillsAsync(string userId,List<int> userSkillsId);
        public Task<Response<IEnumerable<SkillDTO>>> GetUserSkillsAsync(string userId);
    }
}
