using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class Reacts
    {
        public int Id { get; set; }
        public DateTime ReactDate { get; set; }
        public ReactType ReactType { get; set; }

        //Foreign Keys
        public string UserId { get; set; }
        public string PostId { get; set; }

        //Navigation Property

        public virtual ApplicationUser User { get; set; }
        public virtual Post Post { get; set; }
    }
}
