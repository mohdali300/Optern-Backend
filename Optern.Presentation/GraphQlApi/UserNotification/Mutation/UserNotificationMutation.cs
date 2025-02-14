//using Optern.Application.DTOs.UserNotification;

using Optern.Application.DTOs.UserNotification;
using Optern.Application.Interfaces.IUserNotificationService;

namespace Optern.Presentation.GraphQlApi.UserNotification.Mutation
{
    [ExtendObjectType("Mutation")]
    public class UserNotificationMutation
    {
        [GraphQLDescription("Add Notification")]
        public async Task<Response<string>> SaveNotification([Service] IUserNotificationService _notificationService,
            UserNotificationDTO model) =>
            await _notificationService.SaveNotification(model);

        [GraphQLDescription("Delete Notification")]
        public async Task<Response<bool>> DeleteNotification([Service] IUserNotificationService _notificationService,
            UserNotificationDTO model) =>
            await _notificationService.DeleteUserNotification(model);

        [GraphQLDescription("Mark Notification as Read")]
        public async Task<Response<string>> MarkNotificationAsRead([Service] IUserNotificationService _notificationService,
            UserNotificationDTO model) =>
            await _notificationService.MarkNotificationAsRead(model);

    }
}
