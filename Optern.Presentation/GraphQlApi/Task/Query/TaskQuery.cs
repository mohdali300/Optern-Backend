
[ExtendObjectType("Query")]
public class TaskQuery
{
    [GraphQLDescription("Get Recent Tasks for User or room leader")]
    public async Task<Response<IEnumerable<RecentTaskDTO>>> GetRecentTasksAsync([Service] ITaskService _taskService, string userId, string roomId)
        =>await _taskService.GetRecentTasksAsync(userId, roomId);

    [GraphQLDescription("Get tasks summary over all room or sprint")]
    public async Task<Response<TasksSummaryDTO>> GetTasksSummaryAsync([Service] ITaskService _taskService, string filterBy, string? roomId = null, int? sprintId = null)
        =>await _taskService.GetTasksSummaryAsync(filterBy, roomId, sprintId);

    [GraphQLDescription("Get tasks with filters")]
    public async Task<Response<TaskStatusGroupedDTO>> GetTasksWithFiltersAsync([Service] ITaskService _taskService, GetTasksWithFiltersDTO request)
        => await _taskService.GetTasksWithFiltersAsync(request);

    [GraphQLDescription("Get Task Details")]
 
    public async Task<Response<TaskDTO>> GetTaskDetailsAsync([Service] ITaskService _taskService, int taskId, string? userId = null)
        => await _taskService.GetTaskDetailsAsync(taskId, userId);




}

