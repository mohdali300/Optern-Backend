
namespace Optern.Presentation.GraphQlApi.BookMarkedTask.Query
{
    [ExtendObjectType("Query")]
    public class BookMarkedTaskQuery
    {
        [GraphQLDescription("Get all Bookmarked Tasks")]
        public async Task<Response<List<BookMarkedTaskDTO>>> GetAllBookMarks([Service] IBookMarkedTaskService _bookMarkedTask, string userId,string roomId)
            => await _bookMarkedTask.GetAll(userId,roomId);

    }
}
