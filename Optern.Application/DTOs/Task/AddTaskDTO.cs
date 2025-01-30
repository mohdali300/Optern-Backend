using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T = Optern.Domain.Enums;

namespace Optern.Application.DTOs.Task
{
    public class AddTaskDTO
    {

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int SprintId { get; set; }

        [Required]
        public int WorkSpaceId { get; set; }

        [Required]

        public string RoomId {  get; set; }

        [Required]
        public T.TaskStatus Status { get; set; }

        [Required]
        public List<string> AssignedUserIds { get; set; } = new();

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string DueDate { get; set; }
    }
}
