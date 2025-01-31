using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class BookMarkedTask
    {
        public int Id { get; set; }

        //Foreign Keys
        public int UserRoomId { get; set; }
        public int TaskId { get; set; }

        // Navigation Properties
        public virtual UserRoom UserRoom { get; set; }
        public virtual Task Task { get; set; }
    }
}
