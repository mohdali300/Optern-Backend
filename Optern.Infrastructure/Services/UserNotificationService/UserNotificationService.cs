using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Notification;
using Optern.Application.DTOs.UserNotification;
using Optern.Application.Interfaces.IUserNotificationService;
using Optern.Domain.Specifications.NotificationSpecification;

namespace Optern.Application.Services.UserNotificationService
{
    public class UserNotificationService(IUnitOfWork unitOfWork, OpternDbContext context, IUserService userService, INotificationService notificationService,IMapper mapper) : IUserNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IUserService _userService = userService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IMapper _mapper = mapper;

        #region Save Notification
        public async Task<Response<string>> SaveNotification(UserNotificationDTO model)
        {
            if (model == null)
            {
                return Response<string>.Failure("Invalid Model Data", "Invalid Model Data", 400);
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var isUserExist = await _userService.IsUserExist(model.UserId);
                var isNotificationExist = await _notificationService.IsNotificationExist(model.NotificationId);
                if (!isUserExist)
                {
                    return Response<string>.Failure("User Not Found", "User Not Found", 404);
                }
                if (!isNotificationExist)
                {
                    return Response<string>.Failure("Notification Not Exist", "Notification Not Exist", 404);
                }
                var userNotification = new UserNotification()
                {
                    UserId = model.UserId,
                    NotificationId = model.NotificationId,
                    IsRead = false
                };
                await _unitOfWork.UserNotification.AddAsync(userNotification);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return Response<string>.Success("Notification Saved Successfully", "Notification Saved Successfully", 200);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
        #endregion

        #region Delete Notification

        public async Task<Response<bool>> DeleteUserNotification(UserNotificationDTO model)
        {
            if (model == null)
            {
                return Response<bool>.Failure("Invalid Model Data", 400);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                
                var userNotification = await _unitOfWork.UserNotification
                    .GetByExpressionAsync(un => un.UserId == model.UserId && un.NotificationId == model.NotificationId);

                if (userNotification == null)
                {
                    return Response<bool>.Failure(false,"User Notification Not Found", 404);
                }

                await _unitOfWork.UserNotification.DeleteAsync(userNotification);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<bool>.Success(true, "Notification Deleted Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure("There is a server error. Please try again later.", 500);
            }
        }
        #endregion


        #region Mark Notification As Read
        public async Task<Response<string>> MarkNotificationAsRead(UserNotificationDTO model)
        {
            if (model == null)
            {
                return Response<string>.Failure("Invalid Model Data", "Invalid Model Data", 400);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var isUserExist = await _userService.IsUserExist(model.UserId);
                var isNotificationExist = await _notificationService.IsNotificationExist(model.NotificationId);

                if (!isUserExist)
                {
                    return Response<string>.Failure("User Not Found", "User Not Found", 404);
                }

                if (!isNotificationExist)
                {
                    return Response<string>.Failure("Notification Not Exist", "Notification Not Exist", 404);
                }

                var userNotification = await _unitOfWork.UserNotification
                    .GetByExpressionAsync(un => un.UserId == model.UserId && un.NotificationId == model.NotificationId);

                if (userNotification == null)
                {
                    return Response<string>.Failure("User Notification Not Found", "User Notification Not Found", 404);
                }
                if(userNotification.IsRead)
                {
                    return Response<string>.Failure("Notification Already Marked as Read", "Notification Already Marked as Read", 404);

                }

                userNotification.IsRead = true;
                await _unitOfWork.UserNotification.UpdateAsync(userNotification);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<string>.Success("Notification Marked as Read Successfully", "Notification Marked as Read Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }
        #endregion

        #region Get User Notifications (Read - Not Read)
        public async Task<Response<IEnumerable<GetUserNotificationDTO>>> GetUserNotifications(string userId, string roomId, bool? isRead = null,int lastIdx = 0, int limit = 10)
        {
            try
            {
                var isUserExist = await _userService.IsUserExist(userId);

                if (!isUserExist)
                {
                    return Response<IEnumerable<GetUserNotificationDTO>>.Failure(new List<GetUserNotificationDTO>(), "Invalid user ID.", 400);
                }
                var query = _context.UserNotifications
                  .AsNoTracking()
                  .Where(un => un.UserId == userId && un.Notifications.RoomId == roomId);

                if (isRead.HasValue)
                {
                    query = query.Where(n => n.IsRead == isRead.Value);
                }
                var userNotifications = await query
                     .Include(un => un.Notifications) 
                     .OrderByDescending(un => un.Notifications.CreatedTime)
                     .Skip(lastIdx)
                     .Take(limit)
                     .ToListAsync();

                 var notificationDTOs = _mapper.Map<IEnumerable<GetUserNotificationDTO>>(userNotifications);
           
                return notificationDTOs.Any()
                    ? Response<IEnumerable<GetUserNotificationDTO>>.Success(notificationDTOs, "User's room notifications retrieved successfully.", 200)
                    : Response<IEnumerable<GetUserNotificationDTO>>.Failure(new List<GetUserNotificationDTO>(), "No notifications found.", 404);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<GetUserNotificationDTO>>.Failure(new List<GetUserNotificationDTO>(),$"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region Search User Notifications
        public async Task<Response<IEnumerable<GetUserNotificationDTO>>> SearchUserNotifications(SearchUserNotificationsDTO model, int lastIdx = 0, int limit = 10)
        {
            try
            {
                var isValidUser = await _userService.IsUserExist(model.UserId);
                var isValidRoom = await _unitOfWork.Rooms.AnyAsync(r => r.Id == model.RoomId);

                if (!isValidUser || !isValidRoom)
                    return Response<IEnumerable<GetUserNotificationDTO>>.Failure(new List<GetUserNotificationDTO>(), "Invalid user ID or room ID.", 400);

                var query = _context.UserNotifications
                    .AsNoTracking()
                    .Include(un => un.Notifications)
                    .Where(un => un.UserId == model.UserId && un.Notifications.RoomId == model.RoomId);

                if (model.IsRead.HasValue)
                    query = query.Where(un => un.IsRead == model.IsRead.Value);

                var specifications = new List<Specification<UserNotification>>(); 

                if (!string.IsNullOrWhiteSpace(model.Keyword))
                    specifications.Add(new NotificationByKeywordSpecification(model.Keyword));

                if (model.CreatedDate.HasValue)
                    specifications.Add(new NotificationByCreatedDateSpecification(model.CreatedDate, model.isDescending ?? false));

                if (specifications.Any())
                {
                    var combinedSpec = specifications.Aggregate((spec1, spec2) => spec1.And(spec2));
                    query = combinedSpec.Apply(query);
                }

                var notifications = await query
                    .Skip(lastIdx)
                    .Take(limit)
                    .ToListAsync();

                var notificationDTOs = _mapper.Map<IEnumerable<GetUserNotificationDTO>>(notifications);

                return notificationDTOs.Any()
                    ? Response<IEnumerable<GetUserNotificationDTO>>.Success(notificationDTOs, "Notifications retrieved successfully.", 200)
                    : Response<IEnumerable<GetUserNotificationDTO>>.Failure(new List<GetUserNotificationDTO>(), "No notifications found.", 404);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<GetUserNotificationDTO>>.Failure(new List<GetUserNotificationDTO>(), $"Server error: {ex.Message}", 500);
            }
        }

        #endregion




    }
}
