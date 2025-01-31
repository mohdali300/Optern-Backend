using Optern.Application.DTOs.Sprint;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.ISprintService
{
    public interface ISprintService
    {
        public  Task<Response<IEnumerable<SprintResponseDTO>>> GetWorkSpaceSprints(int workSpaceId);
        public Task<Response<SprintResponseDTO>> AddSprint(AddSprintDTO model);
        public Task<Response<SprintResponseDTO>> EditSprint(int id, EditSprintDTO model);
        public Task<Response<bool>> DeleteSprint(int id);
    }
}
