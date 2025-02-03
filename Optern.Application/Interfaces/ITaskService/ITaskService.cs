using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Application.DTOs.Task;
using Optern.Domain.Enums;
using Optern.Infrastructure.Response;

namespace Optern.Application.Interfaces.ITaskService
{
   public interface ITaskService
    {
        public Task<Response<TaskResponseDTO>> AddTaskAsync(AddTaskDTO taskDto, string userId);
        public Task<Response<TaskResponseDTO>> EditTaskAsync(EditTaskDTO editTaskDto, string userId);
        public Task<Response<string>> DeleteTaskAsync(int taskId, string userId);
        public Task<Response<string>> SubmitTaskAsync(int taskId, string userId, IFile? file, TaskState? newStatus);
        public Task<Response<TaskStatusGroupedDTO>> GetTasksWithFiltersAsync(GetTasksWithFiltersDTO request);
        public Task<Response<TaskDTO>> GetTaskDetailsAsync(int taskId);

        public Task<Response<IEnumerable<RecentTaskDTO>>> GetRecentTasksAsync(string userId,string roomId,bool? isAdmin= false);
        public Task<Response<TasksSummaryDTO>> GetTasksSummaryAsync(string filterBy, string? roomId = null, int? sprintId = null);
    }
}
