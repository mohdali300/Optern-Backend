//using Optern.Application.DTOs.UserNotification;
using Optern.Infrastructure.ExternalDTOs.UserNotification;
using Optern.Infrastructure.ExternalInterfaces.IUserNotificationService;

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
    }
}
