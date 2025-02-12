
namespace Optern.Application.Interfaces.IRepositoryService
{
    public interface IRepositoryService
    {
        public Task<Response<bool>> AddRepository(string roomId);
    }
}
