using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.Task;
using Optern.Application.Interfaces.ITaskService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using Optern.Infrastructure.Validations;
using Task = Optern.Domain.Entities.Task;

namespace Optern.Application.Services.TaskService
{
    public class TaskService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        #region Add Task

        public async Task<Response<TaskResponseDTO>> AddTaskAsync(AddTaskDTO taskDto, string userId)
        {
           using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var isAdmin = await _unitOfWork.UserRoom.AnyAsync(ur => ur.UserId == userId && ur.RoomId == taskDto.RoomId && ur.IsAdmin);
                if (!isAdmin)
                {
                    return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(), "Only admins can add tasks.", 403);
                }

                var workspace = await _unitOfWork.WorkSpace.GetByIdAsync((int)taskDto.WorkSpaceId!);
                if (workspace == null || workspace.RoomId != taskDto.RoomId)
                {
                    return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(), "Invalid workspace selection.", 404);
                }

                var sprint = await _unitOfWork.Sprints.GetByIdAsync((int)taskDto.SprintId!);
                if (sprint == null || sprint.WorkSpaceId != taskDto.WorkSpaceId)
                {
                    return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(), "Invalid sprint selection.", 404);
                }

                var userRooms = await _unitOfWork.UserRoom.GetAllByExpressionAsync(ur => ur.RoomId == taskDto.RoomId);
                var roomMembersSet = new HashSet<string>(userRooms.Select(ur => ur.UserId));
                if (!taskDto.AssignedUserIds.All(uid => roomMembersSet.Contains(uid)))
                {
                    return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(), "Some assigned users are not part of the room.", 404);
                }

                var task = _mapper.Map<Task>(taskDto);
                task.AssignedTasks = taskDto.AssignedUserIds.Select(uid => new UserTasks { TaskId = task.Id, UserId = uid , Assignedat=task.CreatedAt }).ToList();

                var validate = new TaskValidator().Validate(task);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.SaveAsync();
               await transaction.CommitAsync();

                var taskWithUsers = await _context.Tasks
                    .Where(t => t.Id == task.Id)
                    .Include(t => t.AssignedTasks)
                    .ThenInclude(at => at.User)
                    .FirstOrDefaultAsync();

                if (taskWithUsers == null)
                {
                    return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(), "Task retrieval failed after creation.", 500);
                }

                var taskResponseDto = _mapper.Map<TaskResponseDTO>(task);
                return Response<TaskResponseDTO>.Success(taskResponseDto, "Task created successfully.", 200);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Response<TaskResponseDTO>.Failure($"Database error: {ex.Message}", 500);
            }
            catch (Exception ex)
            {
               await transaction.RollbackAsync();
                return Response<TaskResponseDTO>.Failure($"Server error: {ex.Message}", 500);
            }
        }


        #endregion

        #region Edit Task

        public async Task<Response<TaskResponseDTO>> EditTaskAsync(EditTaskDTO editTaskDto, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var task = await _unitOfWork.Tasks
                    .GetQueryable(t => t.Id == editTaskDto.TaskId, includeProperties: "AssignedTasks.User")
                    .FirstOrDefaultAsync();

                if (task == null)
                    return Response<TaskResponseDTO>.Failure( new TaskResponseDTO(), "Task not found.", 404);

                var isAdmin = await _unitOfWork.UserRoom.AnyAsync(ur => ur.UserId == userId && ur.RoomId == editTaskDto.RoomId && ur.IsAdmin);

                if (!isAdmin)
                    return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(),"You are not authorized to edit this task.", 403);

                if (editTaskDto.WorkspaceId.HasValue)
                {
                    var workspace = await _unitOfWork.WorkSpace.GetByIdAsync((int)editTaskDto.WorkspaceId);
                    if (workspace == null || workspace.RoomId != editTaskDto.RoomId)
                    {
                        return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(),"Invalid workspace selection.",404);
                    }
                }

                if (editTaskDto.SprintId.HasValue)
                {
                    var isValidSprint = await _unitOfWork.Sprints
                        .AnyAsync(s => s.Id == editTaskDto.SprintId.Value && s.WorkSpaceId == (editTaskDto.WorkspaceId ?? task.Sprint.WorkSpaceId));

                    if (!isValidSprint)
                        return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(),"Invalid sprint.", 400);
                }

                // incase user remain the value as same or empty
                if (!string.IsNullOrEmpty(editTaskDto.Title) && editTaskDto.Title != task.Title)
                    task.Title = editTaskDto.Title;

                if (!string.IsNullOrEmpty(editTaskDto.Description) && editTaskDto.Description != task.Description)
                    task.Description = editTaskDto.Description;

                if (editTaskDto.Status.HasValue && editTaskDto.Status != task.Status)
                    task.Status = editTaskDto.Status.Value;

                if (!string.IsNullOrEmpty(editTaskDto.StartDate) && editTaskDto.StartDate != task.StartDate)
                    task.StartDate = editTaskDto.StartDate;

                if (!string.IsNullOrEmpty(editTaskDto.DueDate) && editTaskDto.DueDate != task.DueDate)
                    task.DueDate = editTaskDto.DueDate;


                if (editTaskDto.AssignedUserIds != null)
                {
                    var validUserIds = await _context.Users
                        .Where(u => editTaskDto.AssignedUserIds.Contains(u.Id))
                        .Select(u => u.Id)
                        .ToListAsync();

                    var invalidUserIds = editTaskDto.AssignedUserIds.Except(validUserIds).ToList();

                    if (invalidUserIds.Any())
                        return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(),$"Invalid user IDs: {string.Join(", ", invalidUserIds)}", 400);

                    // Remove users who are no longer assigned
                    var usersToRemove = task.AssignedTasks
                        .Where(at => !editTaskDto.AssignedUserIds.Contains(at.UserId))
                        .ToList();

                    foreach (var userTask in usersToRemove)
                    {
                        await _unitOfWork.UserTasks.DeleteAsync(userTask);
                    }

                    // Add new assigned users
                    var existingUserIds = task.AssignedTasks.Select(at => at.UserId).ToList();
                    var usersToAdd = editTaskDto.AssignedUserIds
                        .Where(uid => !existingUserIds.Contains(uid))
                        .Select(uid => new UserTasks { TaskId = (int)editTaskDto.TaskId!, UserId = uid , Assignedat=DateTime.UtcNow })
                        .ToList();

                    await _unitOfWork.UserTasks.AddRangeAsync(usersToAdd);
                }

                var validate = new TaskValidator().Validate(task);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.Tasks.UpdateAsync(task);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                var taskResponseDto = _mapper.Map<TaskResponseDTO>(task);
                return Response<TaskResponseDTO>.Success(taskResponseDto, "Task updated successfully.",200);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Response<TaskResponseDTO>.Failure($"Database error: {ex.Message}", 500);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<TaskResponseDTO>.Failure($"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region Delete Task

        public async Task<Response<string>> DeleteTaskAsync(int taskId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var task = await _unitOfWork.Tasks
           .GetQueryable(t => t.Id == taskId, includeProperties: "Sprint.WorkSpace")
           .FirstOrDefaultAsync();

                if (task == null)
                    return Response<string>.Failure("Task not found.", 404);

                var isAdmin = await _unitOfWork.UserRoom.AnyAsync(ur => ur.UserId == userId && ur.RoomId == task.Sprint.WorkSpace.RoomId && ur.IsAdmin);
                if (!isAdmin)
                    return Response<string>.Failure("You are not authorized to delete this task.", 403);


                await _unitOfWork.Tasks.DeleteAsync(task);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<string>.Success("Task deleted successfully.", "Task deleted successfully.", 200);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"Database error: {ex.Message}", 500);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region Get Tasks for each Status (sprint optinal --> get all sprints in that work space and workspace optinal -->get all worksapces tasks with its sprints )
        public async Task<Response<TaskStatusGroupedDTO>> GetTasksByStatusAsync(GetTasksByStatusDTO request)
        {
            try
            {
                if (!await IsUserMemberOfRoomAsync(request.UserId!, request.RoomId!))
                {
                    return Response<TaskStatusGroupedDTO>.Failure(new TaskStatusGroupedDTO(), "You are not authorized to view tasks in this room.", 403);
                }

                var workspace = request.WorkspaceId.HasValue ? await _unitOfWork.WorkSpace.GetByIdAsync(request.WorkspaceId.Value) : null;
                if (workspace == null && request.WorkspaceId.HasValue)
                {
                    return Response<TaskStatusGroupedDTO>.Failure(new TaskStatusGroupedDTO(), "Invalid workspace selection.", 404);
                }

                var sprint = request.SprintId.HasValue ? await _unitOfWork.Sprints.GetByIdAsync(request.SprintId.Value) : null;
                if (sprint == null && request.SprintId.HasValue)
                {
                    return Response<TaskStatusGroupedDTO>.Failure(new TaskStatusGroupedDTO(), "Invalid sprint selection.", 404);
                }

                IQueryable<Task> taskQuery = _context.Tasks
                    .Where(t => t.Sprint.WorkSpace.RoomId == request.RoomId);

                 if (request.WorkspaceId.HasValue)
                {
                    taskQuery = taskQuery.Where(t => t.Sprint.WorkSpaceId == request.WorkspaceId);
                }

                if (request.SprintId.HasValue)
                {
                    taskQuery = taskQuery.Where(t => t.SprintId == request.SprintId);
                }

                taskQuery = taskQuery
                    .Include(t => t.AssignedTasks)
                    .ThenInclude(ut => ut.User)
                    .Include(t => t.Sprint)
                    .ThenInclude(s => s.WorkSpace).OrderByDescending(t => t.CreatedAt);

                var tasks = await taskQuery.ToListAsync();

                if (!tasks.Any())
                {
                    return Response<TaskStatusGroupedDTO>.Failure(new TaskStatusGroupedDTO(), "No tasks found for the given criteria.", 404);
                }

                var mappedTasks = _mapper.Map<List<TaskResponseDTO>>(tasks);

                var groupedTasks = GroupTasksByStatus(mappedTasks);

                return Response<TaskStatusGroupedDTO>.Success(groupedTasks, "Tasks retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<TaskStatusGroupedDTO>.Failure($"Server error: {ex.Message}", 500);
            }
        }

        #endregion

 
        #region Recent Tasks
        public async Task<Response<IEnumerable<RecentTaskDTO>>> GetRecentTasksAsync(string userId, string roomId, bool? isAdmin = false)
        {
            try
            {
                var tasks = await GetRecentTasksForUserAsync(userId, roomId, isAdmin);

                if (tasks.Any())
                {
                    var dto = tasks.Select(t => new RecentTaskDTO
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Status = t.Status,
                        DueDate = t.DueDate,
                    });
                    return Response<IEnumerable<RecentTaskDTO>>.Success(dto);
                }

                return Response<IEnumerable<RecentTaskDTO>>.Success(new List<RecentTaskDTO>(), "No Tasks found in this room.", 204);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RecentTaskDTO>>.Failure($"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region Tasks summary
        public async Task<Response<TasksSummaryDTO>> GetTasksSummaryAsync(string filterBy, string? roomId = null, int? sprintId = null)
        {
            try
            {
                var query = _context.Tasks.Include(t=>t.Sprint)
                    .ThenInclude(s=>s.WorkSpace).AsQueryable();
                if (!string.IsNullOrEmpty(roomId))
                {
                    query = query.Where(t => t.Sprint.WorkSpace.RoomId == roomId);
                }

                if (sprintId.HasValue)
                {
                    query = query.Where(t => t.SprintId == sprintId);
                }

                var tasks = await GetTasksbyFilterAsync(query, filterBy);
                if (tasks.Any())
                {
                    var dto = new TasksSummaryDTO
                    {
                        ToDoTasks = tasks.Where(t => t.Status == TaskState.ToDo).Count(),
                        InProgressTasks = tasks.Where(t => t.Status == TaskState.InProgress).Count(),
                        DoneTasks = tasks.Where(t => t.Status == TaskState.Completed).Count()
                    };

                    return Response<TasksSummaryDTO>.Success(dto);
                }

                return Response<TasksSummaryDTO>.Success(new TasksSummaryDTO
                {
                    ToDoTasks = 0,
                    InProgressTasks = 0,
                    DoneTasks = 0,
                }, "There is no tasks yet.", 204);
            }
            catch (Exception ex)
            {
                return Response<TasksSummaryDTO>.Failure($"Server error: {ex.Message}", 500);
            }
        } 
        #endregion

        #region Helpers
        private async Task<List<Task>> GetRecentTasksForUserAsync(string userId, string roomId, bool? isAdmin)
        {
            var query = _context.Tasks
                .Include(t => t.Sprint)
                .ThenInclude(s => s.WorkSpace)
                .Where(t => t.Sprint.WorkSpace.RoomId == roomId);

            if (isAdmin == true)
            {
                // If user is admin, retrieve the 8 most recent tasks
                return await query
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(8)
                    .ToListAsync();
            }

            // If user is not admin, retrieve tasks assigned to the user
            return await query
                .Join(
                    _context.UserTasks,
                    task => task.Id,
                    userTask => userTask.TaskId,
                    (task, userTask) => new { Task = task, UserTask = userTask }
                )
                .Where(ut => ut.UserTask.UserId == userId)
                .Select(ut => ut.Task).OrderByDescending(t => t.CreatedAt)
                .Take(8).ToListAsync();
        } 

        private async Task<IQueryable<Task>> GetTasksbyFilterAsync(IQueryable<Task> query,string filterBy)
        {
            switch (filterBy)
            {
                case "week":
                    return query.Where(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-7) && t.CreatedAt <= DateTime.UtcNow);
                case "month":
                    return query.Where(t => t.CreatedAt >= DateTime.UtcNow.AddMonths(-1) && t.CreatedAt <= DateTime.UtcNow);
                case "all":
                default: 
                    return query;
            }
        }

        private async Task<bool> IsUserMemberOfRoomAsync(string userId, string roomId)
         {
           return await _unitOfWork.UserRoom.AnyAsync(ur => ur.UserId == userId && ur.RoomId == roomId);
         }

    

     private TaskStatusGroupedDTO GroupTasksByStatus(List<TaskResponseDTO> tasks)
     {
    return new TaskStatusGroupedDTO
    {
        ToDo = tasks.Where(t => t.Status == TaskState.ToDo).ToList(),
        InProgress = tasks.Where(t => t.Status == TaskState.InProgress).ToList(),
        Completed = tasks.Where(t => t.Status == TaskState.Completed).ToList(),
        ToDoCount = tasks.Count(t => t.Status == TaskState.ToDo),
        InProgressCount = tasks.Count(t => t.Status == TaskState.InProgress),
        CompletedCount = tasks.Count(t => t.Status == TaskState.Completed)
    };
     }
        #endregion
    }
}
