using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public TaskStatus Status { get; set; }

        // Foreign Keys
        public int SprintID { get; set; }

        //Navigation Properties

        public virtual ICollection<UserTasks> AssigendTasks { get; set; }
        public virtual Sprint Sprint { get; set; }

    }
}
