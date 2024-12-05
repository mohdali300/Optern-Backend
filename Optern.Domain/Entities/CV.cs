using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class CV
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }

        //Foreign Keys
        public string UserId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; }

    }
}
