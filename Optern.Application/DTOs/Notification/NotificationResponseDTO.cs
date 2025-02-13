using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Notification
{
    public class NotificationResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime  CreatedTime { get; set; }

        public NotificationResponseDTO()
        {
            Id = 0;
            Title=string.Empty;
            Message = string.Empty;
            CreatedTime = DateTime.MinValue;
        }
    }
}
