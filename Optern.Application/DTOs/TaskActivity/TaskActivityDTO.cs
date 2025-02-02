using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.TaskActivity
{
    public class TaskActivityDTO
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string CreatorId { get; set; } = string.Empty;

        public bool CouldDelete { get; set; }

        public string CreatorName { get; set; } = string.Empty;

        public string CreatorProfilePicture { get; set; } = string.Empty;

    }
}
