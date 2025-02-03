using Optern.Application.DTOs.TaskActivity;
using Optern.Application.Interfaces.ITaskActivityService;

namespace Optern.Presentation.GraphQlApi.TaskActivity.Query
{
    [ExtendObjectType("Query")]
    public class TaskActivityQuery
    {
        [GraphQLDescription("Get All Task Activities")]
        public async Task<Response<IEnumerable<TaskActivityDTO>>> GetAllTaskActivitiesAsync([Service] ITaskActivityService taskActivity , int taskid)

      => await taskActivity.GetAllTaskActivitiesAsync(taskid);
    }
}
