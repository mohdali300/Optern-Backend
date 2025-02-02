using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.TaskActivity
{
    public class AddTaskCommentDTO
    {
        public int TaskId { get; set; }
        public string Content { get; set; }
        public string RoomId { get; set; }
    }
}
