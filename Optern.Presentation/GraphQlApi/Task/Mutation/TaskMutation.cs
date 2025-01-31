
    [ExtendObjectType("Mutation")]
    public class TaskMutation
    {
        [GraphQLDescription("Create a new task")]
        public async Task<Response<TaskResponseDTO>> AddTaskAsync(
     [Service] ITaskService taskService,
     string userId,
     AddTaskDTO taskDto)
        
            => await taskService.AddTaskAsync(taskDto, userId);

}
