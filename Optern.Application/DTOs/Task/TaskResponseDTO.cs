using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Domain.Enums;

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
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TaskState Status { get; set; }
        public string StartDate { get; set; }
        public string DueDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public List<AssignedUserDTO> AssignedUsers { get; set; }
    }
}
