using Optern.Application.Interfaces.IBookMarkedTaskService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.BookMarkedTask.Mutation
{
    [ExtendObjectType("Mutation")]
    public class BookMarkedTaskMutation
    {

        [GraphQLDescription("Add Task to Bookmarks")]
        public async Task<Response<string>> AddBookMark([Service] IBookMarkedTaskService _bookMarkedTask, int roomUserId, int taskId)
            => await _bookMarkedTask.Add(roomUserId, taskId);

        [GraphQLDescription("Delete Bookmarked Task")]
        public async Task<Response<string>> DeleteBookMark([Service] IBookMarkedTaskService _bookMarkedTask, int bookMarkId)
            => await _bookMarkedTask.Delete(bookMarkId);
    }
}
