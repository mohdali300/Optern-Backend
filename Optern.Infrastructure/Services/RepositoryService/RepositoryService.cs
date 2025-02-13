



namespace Optern.Infrastructure.Services.RepositoryService
{
    public class RepositoryService(IUnitOfWork unitOfWork, OpternDbContext context) : IRepositoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;

        public async Task<Response<bool>> AddRepository(string roomId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room == null)
            {
                return Response<bool>.Failure(false, "Room Not Found", 400);
            }
            try
            {
                var repo = new Repository
                {
                    RoomId = roomId,
                };
                await _unitOfWork.Repository.AddAsync(repo);
                await _unitOfWork.SaveAsync();
                return Response<bool>.Success(true, "Repository Added Successfully");

            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }
    }
}
