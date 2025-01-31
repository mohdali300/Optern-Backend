using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Task
{
    public class TaskResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public string StartDate { get; set; }
        public string DueDate { get; set; }
        public List<AssignedUserDTO> AssignedUsers { get; set; }
    }
}
