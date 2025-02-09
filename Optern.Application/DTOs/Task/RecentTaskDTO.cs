

namespace Optern.Application.DTOs.Task
{
    public class RecentTaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TaskState Status { get; set; }
        public string DueDate { get; set; }
    }
}
