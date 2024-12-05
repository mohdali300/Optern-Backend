using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class PostTags
    {
        public int Id { get; set; }

        // Foreign Keys
        public int PostId { get; set; }
        public int TagId { get; set; }

        // Navigation Properties

        public virtual Post Post { get; set; }
        public virtual Tags Tag { get; set; }
    }
}
