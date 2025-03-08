using Optern.Application.DTOs.ExperienceDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IExperienceService
{
    public interface IExperienceService
    {
        public Task<Response<bool>> AddExperience(string userId, ExperienceDTO model);
        public Task<Response<IEnumerable<ExperienceDTO>>> GetUserExperiences(string userId);
    }
}
