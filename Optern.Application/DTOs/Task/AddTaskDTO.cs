

namespace Optern.Application.DTOs.Task
{
    public class AddTaskDTO
    {
        public AddTaskDTO()
        {
            Title = string.Empty;  
            Description = string.Empty; 
            SprintId = 0; 
            WorkSpaceId = 0;  
            RoomId = string.Empty;  
            Status = TaskState.ToDo;  
            AssignedUserIds = new List<string>(); 
            StartDate = string.Empty;  
            DueDate = string.Empty;  
        }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int? SprintId { get; set; }

        [Required]
        public int? WorkSpaceId { get; set; }

        [Required]

        public string? RoomId {  get; set; }

        [Required]
        public TaskState? Status { get; set; }

        [Required]
        public List<string> AssignedUserIds { get; set; } = new();

        [Required]
        public string? StartDate { get; set; }

        [Required]
        public string? DueDate { get; set; }
    }
}
