
namespace Optern.Application.Interfaces.IPositionService
{
    public interface IPositionService
    {
        public Task<Response<List<PositionDTO>>> GetAllAsync();
        public Task<Response<PositionDTO>> AddAsync(string name);
    }
}
