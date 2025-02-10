

namespace Optern.Application.Services.TaskActivityService
{
    public class TaskActivityService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : ITaskActivityService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;


        #region Add Comment to Task
        public async Task<Response<TaskActivityDTO>> AddTaskActivityAsync(AddTaskCommentDTO model, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(model.Content))
                {
                    return Response<TaskActivityDTO>.Failure(new TaskActivityDTO(),"Content cannot be empty.",400);
                }

                var room = await _unitOfWork.Rooms.GetByIdAsync(model.RoomId);
                var task = await _unitOfWork.Tasks.GetByIdAsync(model.TaskId);

                if (room == null||task==null)
                    return Response<TaskActivityDTO>.Failure(new TaskActivityDTO(),"Room or Task not found.",404);
              
                var isAdmin = await _unitOfWork.UserRoom
                    .AnyAsync(ur => ur.RoomId == room.Id && ur.UserId == userId && (ur.IsAdmin));
                var isUserAssignedToTask = await _unitOfWork.UserTasks
                 .AnyAsync(ut => ut.TaskId == model.TaskId && ut.UserId == userId);

                if (!isAdmin||!isUserAssignedToTask)
                    return Response<TaskActivityDTO>.Failure(new TaskActivityDTO(),"You do not have permission to add this task Comment.",400);

                var newTaskActivity = new TaskActivity
                {
                    TaskId = model.TaskId,
                    Content = model.Content,
                    CreatedAt = DateTime.UtcNow,
                    CouldDelete = true,
                    CreatorId = userId
                };

                var validate = new TaskActivityValidator().Validate(newTaskActivity);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<TaskActivityDTO>.Failure(new TaskActivityDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.TaskActivites.AddAsync(newTaskActivity);
                await _unitOfWork.SaveAsync();

                var taskActivity = await _context.TaskActivities
                  .Include(ta => ta.Creator) 
                  .FirstOrDefaultAsync(ta => ta.Id == newTaskActivity.Id);

                if (taskActivity == null)
                    return Response<TaskActivityDTO>.Failure("Task activity not found.");

                var taskActivityDto = _mapper.Map<TaskActivityDTO>(taskActivity);

                await transaction.CommitAsync();

                return Response<TaskActivityDTO>.Success(taskActivityDto, "Task activity added successfully.",200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<TaskActivityDTO>.Failure($"Error adding task activity: {ex.Message}");
            }
        }
        #endregion

        #region Edit Comment in Task
        public async Task<Response<TaskActivityDTO>> EditTaskActivityAsync(int taskActivityId, string newContent, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(newContent))
                {
                    return Response<TaskActivityDTO>.Failure(new TaskActivityDTO(), "Content cannot be empty.", 400);
                }

                var taskActivity = await _context.TaskActivities
                    .Include(ta => ta.Creator) 
                    .FirstOrDefaultAsync(ta => ta.Id == taskActivityId);

                if (taskActivity == null)
                {
                    return Response<TaskActivityDTO>.Failure(new TaskActivityDTO(),"Task activity not found.", 404);
                }
                var isAdmin = await _unitOfWork.UserRoom
                    .AnyAsync(ur => ur.UserId == userId && ur.IsAdmin);

                if (taskActivity.CreatorId != userId && !isAdmin)
                {
                    return Response<TaskActivityDTO>.Failure(new TaskActivityDTO(),"You do not have permission to edit this task activity.", 403);
                }

                if (!taskActivity.CouldDelete)
                {
                    return Response<TaskActivityDTO>.Failure(new TaskActivityDTO(),"This task activity cannot be edited.", 400);
                }

                taskActivity.Content = newContent;

                var validate = new TaskActivityValidator().Validate(taskActivity);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<TaskActivityDTO>.Failure(new TaskActivityDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.TaskActivites.UpdateAsync(taskActivity);
                await _unitOfWork.SaveAsync();

                var taskActivityDto = _mapper.Map<TaskActivityDTO>(taskActivity);

                await transaction.CommitAsync();

                return Response<TaskActivityDTO>.Success(taskActivityDto, "Task activity updated successfully.", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<TaskActivityDTO>.Failure($"Error updating task activity: {ex.Message}");
            }
        }
        #endregion

        #region Delete Comment in Task

        public async Task<Response<string>> DeleteTaskActivityAsync(int taskActivityId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var taskActivity = await _context.TaskActivities
                    .Include(ta => ta.Creator) 
                    .FirstOrDefaultAsync(ta => ta.Id == taskActivityId);

                if (taskActivity == null)
                {
                    return Response<string>.Failure("","Task activity not found.", 404);
                }

                var isAdmin = await _unitOfWork.UserRoom
                    .AnyAsync(ur => ur.UserId == userId && ur.IsAdmin);

                if (taskActivity.CreatorId != userId && !isAdmin)
                {
                    return Response<string>.Failure("","You do not have permission to delete this task activity.", 403);
                }

                if (!taskActivity.CouldDelete)
                {
                    return Response<string>.Failure("","This task activity cannot be deleted.", 400);
                }

               await _unitOfWork.TaskActivites.DeleteAsync(taskActivity);
                await _unitOfWork.SaveAsync();

                await transaction.CommitAsync();

                return Response<string>.Success("","Task activity deleted successfully.", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"Error deleting task activity: {ex.Message}", 500);
            }
        }


        #endregion

        #region Get all Task Activity

        public async Task<Response<IEnumerable<TaskActivityDTO>>> GetAllTaskActivitiesAsync(int taskId)
        {
            try
            {
                var taskExists = await _unitOfWork.Tasks.AnyAsync(t => t.Id == taskId);

                if (!taskExists)
                {
                    return Response<IEnumerable<TaskActivityDTO>>.Failure(new List<TaskActivityDTO>(), "Task not found.", 404);
                }
                var taskActivities = await _context.TaskActivities
                    .Where(ta => ta.TaskId == taskId) 
                    .Include(ta => ta.Creator) 
                    .OrderBy(ta => ta.CreatedAt) 
                    .ToListAsync();

                if (taskActivities == null || taskActivities.Count == 0)
                {
                    return Response<IEnumerable<TaskActivityDTO>>.Failure(new List<TaskActivityDTO>(), "No task activities found for the specified task.", 404);
                }

                var taskActivityDtos = _mapper.Map<List<TaskActivityDTO>>(taskActivities);

                return Response<IEnumerable<TaskActivityDTO>>.Success(taskActivityDtos, "Task activities retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<TaskActivityDTO>>.Failure($"Error retrieving task activities: {ex.Message}", 500);
            }
        }

       

        #endregion


    }
}
