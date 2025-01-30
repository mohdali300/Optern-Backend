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
                    return Response<TaskResponseDTO>.Failure("Only admins can add tasks.", 403);
                }

                var workspace = await _unitOfWork.WorkSpace.GetByIdAsync(taskDto.WorkSpaceId);
                if (workspace == null || workspace.RoomId != taskDto.RoomId)
                {
                    return Response<TaskResponseDTO>.Failure("Invalid workspace selection.");
                }

                var sprint = await _unitOfWork.Sprints.GetByIdAsync(taskDto.SprintId);
                if (sprint == null || sprint.WorkSpaceId != taskDto.WorkSpaceId)
                {
                    return Response<TaskResponseDTO>.Failure("Invalid sprint selection.");
                }


                var userRooms = await _unitOfWork.UserRoom.GetAllByExpressionAsync(ur => ur.RoomId == taskDto.RoomId);
                var roomMembers = userRooms.Select(ur => ur.UserId).ToList();



                if (!taskDto.AssignedUserIds.All(uid => roomMembers.Contains(uid)))
                {
                    return Response<TaskResponseDTO>.Failure("Some assigned users are not part of the room.");
                }

                var task = new Task
                {
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    StartDate = taskDto.StartDate,  
                    DueDate = taskDto.DueDate,
                    Status = taskDto.Status,
                    SprintId = taskDto.SprintId,
                    EndDate = "--",
                    AssignedTasks = taskDto.AssignedUserIds.Select(uid => new UserTasks { UserId = uid }).ToList()
                };

                var validate = new TaskValidator().Validate(task);

                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<TaskResponseDTO>.Failure(new TaskResponseDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                var taskResponseDto = new TaskResponseDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = (TaskStatus)task.Status,
                    StartDate = task.StartDate,
                    DueDate = task.DueDate,
                    AssignedUsers = task.AssignedTasks.Select(at => new AssignedUserDTO
                    {
                        UserId = at.UserId,
                        FullName = $"{at.User.FirstName} {at.User.LastName}",
                        ProfilePicture = at.User.ProfilePicture
                    }).ToList()
                };

                return Response<TaskResponseDTO>.Success(taskResponseDto, "Task created successfully.");
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
    }
}
