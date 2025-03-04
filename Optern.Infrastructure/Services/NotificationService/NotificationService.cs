using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Notification;
using Optern.Domain.Entities;

namespace Optern.Application.Services.NotificationService
{
    public class NotificationService(IUnitOfWork unitOfWork, OpternDbContext context, IRoomService roomService, IMapper _mapper) : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IRoomService _roomService = roomService;
        private readonly IMapper _mapper = _mapper;

        #region Add Notification
        public async Task<Response<NotificationResponseDTO>> AddNotification(NotificationDTO model)
        {
            if (model == null)
            {
                return Response<NotificationResponseDTO>.Failure(new NotificationResponseDTO(), "Invalid Model Data", 400);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var notification = new Notifications
                {
                    Title = model.Title??null,
                    Message = model.Message,
                    CreatedTime = DateTime.UtcNow,
                    RoomId = model.RoomId??null,
                    Url = model.Url
                };

                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                var notificationDto = _mapper.Map<NotificationResponseDTO>(notification);
                return Response<NotificationResponseDTO>.Success(notificationDto, "Notification Added Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return Response<NotificationResponseDTO>.Failure("There is a server error. Please try again later.", 500);
            }
        }
        #endregion

        #region Is Notification Exist
        public async Task<bool> IsNotificationExist(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            return notification == null ? false : true;
        }
        #endregion
    }
}
