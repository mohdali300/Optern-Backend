using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
