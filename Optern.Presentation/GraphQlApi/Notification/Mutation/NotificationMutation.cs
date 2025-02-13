using Optern.Application.DTOs.Notification;
using Optern.Application.Interfaces.INotificationService;

namespace Optern.Presentation.GraphQlApi.Notification.Mutation
{
    [ExtendObjectType("Mutation")]
    public class NotificationMutation
    {
        [GraphQLDescription("Add Notification")]
        public async Task<Response<NotificationResponseDTO>> AddNotification([Service] INotificationService _notificationService,
            NotificationDTO model) =>
            await _notificationService.AddNotification(model);
    }
}
