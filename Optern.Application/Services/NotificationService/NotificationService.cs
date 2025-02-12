using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optern.Domain.Entities;
using Optern.Infrastructure.ExternalDTOs.Notification;
using Optern.Infrastructure.ExternalInterfaces.INotificationService;

namespace Optern.Application.Services.NotificationService
{
    public class NotificationService(IUnitOfWork unitOfWork, OpternDbContext context, IRoomService roomService) : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IRoomService _roomService = roomService;
        public async Task<Response<bool>> AddNotification(NotificationDTO model)
        {
            if (model == null)
            {
                return Response<bool>.Failure(false,"Invalid Model Data", 400);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var isRoomExist = await _roomService.IsRoomExist(model.RoomId);
                if (!isRoomExist)
                {
                    return Response<bool>.Failure(false,"Room Not Found", 404);

                }
                var notification = new Notifications
                {
                    Title = model.Title,
                    Message = model.Message,
                    CreatedTime = DateTime.UtcNow,
                    RoomId = model.RoomId
                };

                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<bool>.Success(true, "Notification Added Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return Response<bool>.Failure("There is a server error. Please try again later.", 500);
            }
        }


        public async Task<bool> IsNotificationExist(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            return notification == null ? false : true;
        }
    }
}
