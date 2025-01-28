using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Domain.Enums;

namespace Optern.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content {  get; set; }
        public ContentType ContentType { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime EditedDate { get; set; }

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
