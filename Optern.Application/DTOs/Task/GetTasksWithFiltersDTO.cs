

namespace Optern.Application.DTOs.Task
{
    public class GetTasksWithFiltersDTO
    {
        public string RoomId { get; set; }
        public int? WorkspaceId { get; set; }
        public int? SprintId { get; set; }
        public string? AssigneeId { get; set; }
        public string? DueDate { get; set; }
        public string? StartDate { get; set; }
        public string? UserId { get; set; }
    }
}
