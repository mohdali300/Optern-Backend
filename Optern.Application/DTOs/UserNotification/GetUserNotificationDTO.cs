
namespace Optern.Application.DTOs.UserNotification
{
    public class GetUserNotificationDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsRead { get; set; }

        public string url {get;set;}

        public GetUserNotificationDTO()
        {
            Message = string.Empty;
            Title = string.Empty;
            url = string.Empty;
        }
    }


}
