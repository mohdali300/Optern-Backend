using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }
        public ContentType Type { get; set; }

        //Foreign Keys
        public string UserId { get; set; }
        public int PostId { get; set; }
        public int ? ParentId { get; set; }

        //Navigation Property

        public virtual ApplicationUser User { get; set; }
        public virtual Post Post { get; set; }
        public virtual Comment comment { get; set; }
        public virtual ICollection<CommentReacts> CommentReacts { get; set; }
    }
}
