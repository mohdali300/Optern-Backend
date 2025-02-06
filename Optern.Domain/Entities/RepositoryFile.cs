using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class RepositoryFile
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string PublicId { get; set; }

        //Foreign keys
        public int RepositoryId { get; set; }

        // Navigation Prop
        public virtual Repository Repository { get; set; }
    }
}
