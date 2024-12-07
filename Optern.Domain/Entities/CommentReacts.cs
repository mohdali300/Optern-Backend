using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class CommentReacts
    {
        public int Id { get; set; }
        public ReactType ReactType { get; set; }

        // Foreign Keys
        public int CommentId { get; set; }
        public string UserId { get; set; }
        // Navigation Properties
        public virtual Comment Comment { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
