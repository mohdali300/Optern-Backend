
namespace Optern.Infrastructure.Services.BookMarkedTaskService
{
    public class BookMarkedTaskService : IBookMarkedTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OpternDbContext _context;

        public BookMarkedTaskService(IUnitOfWork unitOfWork, OpternDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region Add
        public async Task<Response<string>> Add(string userId, int taskId)
        {
            try
            {
                var bookMarkedTask = await _context.BookMarkedTasks
                    .Where(b => b.UserId == userId && b.TaskId == taskId).FirstOrDefaultAsync();
                if (bookMarkedTask != null)
                {
                    return Response<string>.Failure(string.Empty,"Task is already in your BookMarks.", 400);
                }

                var bookmark = new BookMarkedTask
                {
                    UserId = userId,
                    TaskId = taskId
                };

                await _unitOfWork.BookMarkedTask.AddAsync(bookmark);

                return Response<string>.Success("Task Added to BookMarks successfully.");
            }
            catch (Exception ex)
            {
                return Response<string>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }
        #endregion

        #region Delete
        public async Task<Response<string>> Delete(int bookMarkId)
        {
            try
            {
                var bookmark = await _unitOfWork.BookMarkedTask.GetByIdAsync(bookMarkId);
                if (bookmark != null)
                {
                    await _unitOfWork.BookMarkedTask.DeleteAsync(bookmark);
                    return Response<string>.Success("Task Removed from BookMarks.");
                }
                return Response<string>.Failure(string.Empty, "This Task already is not in your BookMarks.", 400);
            }
            catch (Exception ex)
            {
                return Response<string>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }
        #endregion

        #region Get All
        public async Task<Response<List<BookMarkedTaskDTO>>> GetAll(string userId, string roomId)
        {
            try
            {
                var bookMarks = await _context.BookMarkedTasks.Include(b => b.Task)
                    .ThenInclude(t=>t.Sprint).ThenInclude(s=>s.WorkSpace)
                    .Where(b => b.UserId == userId && b.Task.Sprint.WorkSpace.RoomId == roomId)
                    .ToListAsync();

                if (bookMarks != null && bookMarks.Any())
                {
                    var dto = bookMarks.Select(b => new BookMarkedTaskDTO
                    {
                        Id = b.Id,
                        TaskId = b.TaskId,
                        Title = b.Task.Title,
                        Status = b.Task.Status,
                        DueDate = b.Task.DueDate,
                    }).ToList();

                    return Response<List<BookMarkedTaskDTO>>.Success(dto);
                }
                return Response<List<BookMarkedTaskDTO>>.Success(new List<BookMarkedTaskDTO>(), "There is no BookMarked Tasks yet!", 204);
            }
            catch (Exception ex)
            {
                return Response<List<BookMarkedTaskDTO>>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        } 
        #endregion
    }
}
