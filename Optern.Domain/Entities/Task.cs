using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T = Optern.Domain.Enums;

namespace Optern.Domain.Entities
{
    public class Task
    {
        public int Id { get; set; } 
        public string Title { get; set; } 
        public string Description { get; set; } 
        public string StartDate { get; set; } 
        public string DueDate { get; set; } 
        public string EndDate { get; set; } 
        public TaskState Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int SprintId { get; set; }

        //Navigation Properties

        public virtual ICollection<UserTasks> AssignedTasks { get; set; }
        public virtual Sprint Sprint { get; set; }
        public virtual ICollection<BookMarkedTask> BookMarkedTasks { get; set; }


    }
}
