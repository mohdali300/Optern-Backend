using Optern.Application.DTOs.WorkSpace;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IWorkSpaceService
{
    public interface IWorkSpaceService
    {
        public Task<Response<WorkSpaceDTO>> CreateWorkSpace(WorkSpaceDTO model);
        public Task<Response<WorkSpaceDTO>> UpdateWorkSpace(int id, string title);
        public Task<Response<bool>> DeleteWorkSpace(int id);
    }
}
