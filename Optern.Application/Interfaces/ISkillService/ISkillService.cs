

namespace Optern.Application.Interfaces.ISkillService
{
    public interface ISkillService
    {
        public Task<Response<IEnumerable<SkillDTO>>> AddSkills(IEnumerable<SkillDTO> models);
        public Task<Response<bool>> DeleteSkill(int skillId);
        public Task<Response<List<string>>> GetSkillSuggestions(string word);
    }
}
