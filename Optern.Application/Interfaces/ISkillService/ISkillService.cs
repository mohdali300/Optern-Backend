using Optern.Application.DTOs.Skills;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.ISkillService
{
    public interface ISkillService
    {
        public Task<Response<IEnumerable<SkillDTO>>> AddSkills(IEnumerable<SkillDTO> models);
        public Task<Response<bool>> DeleteSkill(int skillId);
        public Task<Response<List<string>>> GetSkillSuggestions(string word);
    }
}
