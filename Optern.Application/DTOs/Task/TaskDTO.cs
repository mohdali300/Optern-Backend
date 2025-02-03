using Optern.Application.DTOs.TaskActivity;
using Optern.Domain.Enums;

namespace Optern.Application.DTOs.Task
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public TaskState Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<AssignedUserDTO> AssignedUsers { get; set; } = new();
        public List<AttachmentDTO> Attachments { get; set; } = new();

        public List<TaskActivityDTO> Activities { get; set; } = new();

        public TaskDTO() { }
    }


}
