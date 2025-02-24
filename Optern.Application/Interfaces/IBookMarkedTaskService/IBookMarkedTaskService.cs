
namespace Optern.Application.Interfaces.IBookMarkedTaskService
{
    public interface IBookMarkedTaskService
    {
        public Task<Response<List<BookMarkedTaskDTO>>> GetAll(string userId,string roomId);
        public Task<Response<string>> Add(string userId, int taskId);
        public Task<Response<string>> Delete(string userId, int taskId);
    }
}
