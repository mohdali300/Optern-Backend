using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.FavoritePosts
{
    public class AddToFavoriteDTO
    {
        public string UserId { get; set; }
        public int PostId { get; set; }

    }
}
