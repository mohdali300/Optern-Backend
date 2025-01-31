using Optern.Application.DTOs.BookMarkedTask;
using Optern.Application.Interfaces.IBookMarkedTaskService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.BookMarkedTask.Query
{
    [ExtendObjectType("Query")]
    public class BookMarkedTaskQuery
    {
        [GraphQLDescription("Get all Bookmarked Tasks")]
        public async Task<Response<List<BookMarkedTaskDTO>>> GetAllBookMarks([Service] IBookMarkedTaskService _bookMarkedTask, string userId)
            => await _bookMarkedTask.GetAll(userId);

    }
}
