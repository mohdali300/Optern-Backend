using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Task
{
    public class TasksSummaryDTO
    {
        public int ToDoTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int DoneTasks { get; set; }
    }
}
