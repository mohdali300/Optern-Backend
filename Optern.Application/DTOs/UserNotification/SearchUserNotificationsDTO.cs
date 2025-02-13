
namespace Optern.Application.DTOs.UserNotification
{
    public class SearchUserNotificationsDTO
    {
        public string UserId { get; set; }
        public string RoomId { get; set; }
        public string? Keyword { get; set; } =string.Empty; 
        public DateTime? CreatedDate { get; set; }
        public bool? isDescending { get; set; }
        public bool? IsRead { get; set; } 
    }
}
