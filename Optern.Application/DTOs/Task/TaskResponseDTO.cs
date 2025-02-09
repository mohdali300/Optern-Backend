

namespace Optern.Application.DTOs.Task
{
    public class TaskResponseDTO
    {

        public TaskResponseDTO()
        {
            Id = 0;
            Title = string.Empty;
            Description = string.Empty;
            Status = TaskState.ToDo;
            StartDate = string.Empty;
            DueDate = string.Empty;
            CreatedAt = DateTime.UtcNow;
            AssignedUsers = new List<AssignedUserDTO>();
            WorkspaceName = string.Empty;
            SprintName = string.Empty;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TaskState Status { get; set; }
        public string StartDate { get; set; }
        public string DueDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public List<AssignedUserDTO> AssignedUsers { get; set; }

        public int WorkspaceId { get; set; }
        public string? WorkspaceName { get; set; }
        public int SprintId { get; set; }
        public string? SprintName { get; set; }
    }
}
