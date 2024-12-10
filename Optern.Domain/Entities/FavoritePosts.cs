using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class FavoritePosts
    {
        public int Id { get; set; }

        //Foreign Keys
        public string UserId { get; set; }
        public int PostId { get; set; }

        //Navigation Property

        public virtual ApplicationUser User { get; set; }
        public virtual Post Post { get; set; }
    }
}
