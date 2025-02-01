
[ExtendObjectType("Query")]
public class TaskQuery
{
    [GraphQLDescription("Get Recent Tasks for User or room leader")]
    public async Task<Response<IEnumerable<RecentTaskDTO>>> GetRecentTasksAsync([Service] ITaskService _taskService, string userId, string roomId, bool? isAdmin = false)
        =>await _taskService.GetRecentTasksAsync(userId, roomId, isAdmin);
}

