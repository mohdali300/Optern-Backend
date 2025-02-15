using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Message
{
    public class MessageDTO
    {
        public MessageDTO()
        {
            Id= 0;
            Content=string.Empty;
            SentAt= DateTime.UtcNow;   
            SenderId=string.Empty;
            UserName=string.Empty;
            ProfilePicture=string.Empty;
            AttachmentUrl=string.Empty;
            
        }
        public int? Id { get; set; }
        public string? Content { get; set; }
        public DateTime SentAt { get; set; }
        public string? SenderId { get; set; }
        public int? ChatId { get; set; }
        public string? UserName { get;set; }
        public string? ProfilePicture { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public string SentHour => SentAt.ToLocalTime().ToString("hh:mm tt");
        
    }
}
