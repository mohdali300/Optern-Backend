
    [ExtendObjectType("Mutation")]
    public class TaskMutation
    {
        [GraphQLDescription("Create a new task")]
        public async Task<Response<TaskResponseDTO>> AddTaskAsync(
     [Service] ITaskService taskService,
     string userId,
     AddTaskDTO taskDto)
        
            => await taskService.AddTaskAsync(taskDto, userId);

    [GraphQLDescription("Edit task")]
    public async Task<Response<TaskResponseDTO>> EditTaskAsync(
    [Service] ITaskService taskService,
    EditTaskDTO taskDto, string userId)

           => await taskService.EditTaskAsync(taskDto, userId);

    [GraphQLDescription("Delete task")]
    public async Task<Response<string>> DeleteTaskAsync(
   [Service] ITaskService taskService,
   int taskId, string userId)

          => await taskService.DeleteTaskAsync(taskId, userId);


    [GraphQLDescription("Sumbit task")]
    public async Task<Response<string>> SubmitTaskAsync([Service] ITaskService taskService,int taskId, string userId, IFile? file, TaskState? newStatus)
       => await taskService.SubmitTaskAsync(taskId,userId,file,newStatus);

}
