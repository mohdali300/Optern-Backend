using Optern.Application.DTOs.Skills;

namespace Optern.Presentation.GraphQlApi.Skill.Mutation
{
    [ExtendObjectType("Mutation")]
    public class SkillMutation
    {

        public async Task<Response<IEnumerable<SkillDTO>>> AddSkills([Service] ISkillService _skillService,
            IEnumerable<SkillDTO> models) =>
            await _skillService.AddSkills(models); 
        
        public async Task<Response<bool>> DeleteSkill([Service] ISkillService _skillService,
           int skillId) =>
            await _skillService.DeleteSkill(skillId);
    }
}
