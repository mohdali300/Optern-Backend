using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class TaskActivity
    {
        public int Id { get; set; } 
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool CouldDelete { get; set; } 

        // Foreign Key
        public int TaskId { get; set; }
        public Task? Task { get; set; }
    }
}
