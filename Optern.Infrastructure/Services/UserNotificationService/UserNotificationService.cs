using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.UserNotification;
using Optern.Application.Interfaces.IUserNotificationService;

namespace Optern.Application.Services.UserNotificationService
{
    public class UserNotificationService(IUnitOfWork unitOfWork, OpternDbContext context, IUserService userService, INotificationService notificationService) : IUserNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IUserService _userService = userService;
        private readonly INotificationService _notificationService = notificationService;
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

    }
}
