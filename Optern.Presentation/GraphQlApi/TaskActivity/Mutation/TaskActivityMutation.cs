using Optern.Application.DTOs.TaskActivity;
using Optern.Application.Interfaces.ITaskActivityService;

namespace Optern.Presentation.GraphQlApi.TaskActivity.Mutation
{
    [ExtendObjectType("Mutation")]
    public class TaskActivityMutation
    {
        [GraphQLDescription("Create Comment on Task")]

        public async Task<Response<TaskActivityDTO>> AddTaskActivityAsync([Service] ITaskActivityService taskActivity, AddTaskCommentDTO model, string userId)
            => await taskActivity.AddTaskActivityAsync(model, userId);

        [GraphQLDescription("Edit Comment on Task")]
        public async Task<Response<TaskActivityDTO>> EditTaskActivityAsync([Service] ITaskActivityService taskActivity, int taskActivityId, string newContent, string userId)
            => await taskActivity.EditTaskActivityAsync(taskActivityId, newContent, userId);

        [GraphQLDescription("Delete Comment on Task")]

        public async Task<Response<string>> DeleteTaskActivityAsync([Service] ITaskActivityService taskActivity, int taskActivityId, string userId)
            => await taskActivity.DeleteTaskActivityAsync(taskActivityId, userId);


    }


}
