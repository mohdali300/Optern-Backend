
namespace Optern.Application.DTOs.UserNotification
{
    public class SearchUserNotificationsDTO
    {
        public string UserId { get; set; }
        public string? Keyword { get; set; } =string.Empty; 
        public bool? IsRead { get; set; } 
    }
}
