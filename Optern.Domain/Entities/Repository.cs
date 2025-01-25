using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class Repository
    {
        public int Id { get; set; }

        // Foreign Keys
        public string RoomId { get; set; }

        // Navigation Properties
        public virtual Room Room { get; set; }
        public virtual ICollection<RepositoryFile> RepositoryFiles { get; set; }

    }
}
