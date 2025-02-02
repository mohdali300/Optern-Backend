using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class UserTasks
    {
        public int Id {get; set;} 

        // Foreign Keys
        public string UserId {get; set;}
        public int TaskId {get; set;}

        public DateTime Assignedat { get; set; } = DateTime.UtcNow;

        public string AttachmentUrls { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ApplicationUser User {get;set;}
        public virtual Task Task {get;set;}

        [NotMapped]
        public List<string> AttachmentUrlsList
        {
            get => string.IsNullOrEmpty(AttachmentUrls) ? new List<string>() : AttachmentUrls.Split(',').ToList();
            set => AttachmentUrls = value != null ? string.Join(",", value) : string.Empty;
        }
    }
}
