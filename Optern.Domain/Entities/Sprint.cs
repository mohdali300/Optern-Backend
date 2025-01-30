using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class Sprint
    {
        public int Id { get; set;}
        public string Title { get; set;}
        public DateTime StartDate { get; set;}
        public DateTime EndDate { get; set;}
        public string Goal { get; set;}
        // Foreign Keys
        public int WorkSpaceId { get; set;}

        // Navigation Properties
        public virtual WorkSpace WorkSpace { get; set;}
        public virtual ICollection<Task> Tasks{ get; set;}

    }
}
