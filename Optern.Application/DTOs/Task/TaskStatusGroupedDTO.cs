using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Task
{
    public class TaskStatusGroupedDTO
    {
        public List<TaskResponseDTO> ToDo { get; set; } = new();
        public List<TaskResponseDTO> InProgress { get; set; } = new();
        public List<TaskResponseDTO> Completed { get; set; } = new();

        public int ToDoCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
    }
}
