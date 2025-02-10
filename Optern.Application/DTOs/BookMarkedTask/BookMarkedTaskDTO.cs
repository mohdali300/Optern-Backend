

namespace Optern.Application.DTOs.BookMarkedTask
{
    public class BookMarkedTaskDTO
    {
        public BookMarkedTaskDTO()
        {
            Id = 0;
            TaskId = 0;
            Title= string.Empty;
            Status = 0;
            DueDate = string.Empty;
        }

        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Title { get; set; }
        public TaskState Status { get; set; }
        public string DueDate { get; set; }

    }
}
