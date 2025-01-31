using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Application.DTOs.Task;
using Optern.Infrastructure.Response;

namespace Optern.Application.Interfaces.ITaskService
{
   public interface ITaskService
    {
      public Task<Response<TaskResponseDTO>> AddTaskAsync(AddTaskDTO taskDto, string userId);
    }
}
