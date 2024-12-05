using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        //Foreign Keys
        public string CreatorId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser Creator { get; set; }
        public virtual ICollection<Reacts> Reacts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FavoritePosts> FavoritePosts { get; set; }
        public virtual ICollection<PostTags> PostTags { get; set; }

    }
}
