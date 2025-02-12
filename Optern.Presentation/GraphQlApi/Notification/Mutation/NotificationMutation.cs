using Optern.Infrastructure.ExternalInterfaces.INotificationService;
using Optern.Infrastructure.ExternalDTOs.Notification;
using Optern.Infrastructure.ExternalInterfaces.INotificationService;

namespace Optern.Presentation.GraphQlApi.Notification.Mutation
{
    [ExtendObjectType("Mutation")]
    public class NotificationMutation
    {
        [GraphQLDescription("Add Notification")]
        public async Task<Response<bool>> AddNotification([Service] INotificationService _notificationService,
            NotificationDTO model) =>
            await _notificationService.AddNotification(model);
    }
}
