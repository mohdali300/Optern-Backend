using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Task
{
    public class GetTasksByStatusDTO
    {
        public int? SprintId { get; set; }
        public int? WorkspaceId { get; set; }
        public string? RoomId { get; set; }
        public string? UserId { get; set; }
    }
}
