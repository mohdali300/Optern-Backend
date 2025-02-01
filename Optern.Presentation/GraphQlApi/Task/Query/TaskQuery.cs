
[ExtendObjectType("Query")]
public class TaskQuery
{
    [GraphQLDescription("Get Recent Tasks for User or room leader")]
    public async Task<Response<IEnumerable<RecentTaskDTO>>> GetRecentTasksAsync([Service] ITaskService _taskService, string userId, string roomId, bool? isAdmin = false)
        =>await _taskService.GetRecentTasksAsync(userId, roomId, isAdmin);

    [GraphQLDescription("Get tasks summary over all room or sprint")]
    public async Task<Response<TasksSummaryDTO>> GetTasksSummaryAsync([Service] ITaskService _taskService, string filterBy, string? roomId = null, int? sprintId = null)
        =>await _taskService.GetTasksSummaryAsync(filterBy, roomId, sprintId);

    [GraphQLDescription("Get tasks for each Status")]

    public async Task<Response<TaskStatusGroupedDTO>> GetTasksByStatusAsync([Service] ITaskService _taskService, GetTasksByStatusDTO request)
        => await _taskService.GetTasksByStatusAsync(request);

}

