
namespace Optern.Presentation.GraphQlApi.BookMarkedTask.Mutation
{
    [ExtendObjectType("Mutation")]
    public class BookMarkedTaskMutation
    {

        [GraphQLDescription("Add Task to Bookmarks")]
        public async Task<Response<string>> AddBookMark([Service] IBookMarkedTaskService _bookMarkedTask, string userId, int taskId)
            => await _bookMarkedTask.Add(userId, taskId);

        [GraphQLDescription("Delete Bookmarked Task")]
        public async Task<Response<string>> DeleteBookMark([Service] IBookMarkedTaskService _bookMarkedTask, string userId,int taskId)
            => await _bookMarkedTask.Delete(userId,taskId);
    }
}
