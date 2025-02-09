

namespace Optern.Application.DTOs.Task
{
    public class EditTaskDTO
    {
        public EditTaskDTO()
        {
            TaskId = null;  
            Title = string.Empty;  
            Description = string.Empty; 
            Status = null;  
            WorkspaceId = null;  
            SprintId = null;
            StartDate = string.Empty; 
            DueDate = string.Empty;  
            RoomId = string.Empty;  
            AssignedUserIds = new List<string>();  
        }
        public int? TaskId { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskState? Status { get; set; }
        public int? WorkspaceId { get; set; }
        public int? SprintId { get; set; }
        public string? StartDate { get; set; }
        public string? DueDate { get; set; }
        public string? RoomId { get; set; }
        public List<string>? AssignedUserIds { get; set; }
    }
}
