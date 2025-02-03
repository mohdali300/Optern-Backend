
using Optern.Application.DTOs.TaskActivity;
using Optern.Infrastructure.Response;

namespace Optern.Application.Interfaces.ITaskActivityService
{
    public interface ITaskActivityService
    {
        public Task<Response<TaskActivityDTO>> AddTaskActivityAsync(AddTaskCommentDTO model, string userId);
        public Task<Response<TaskActivityDTO>> EditTaskActivityAsync(int taskActivityId, string newContent, string userId);
        public Task<Response<string>> DeleteTaskActivityAsync(int taskActivityId, string userId);
        public Task<Response<IEnumerable<TaskActivityDTO>>> GetAllTaskActivitiesAsync(int taskId);




    }
}
