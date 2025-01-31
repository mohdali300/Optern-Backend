using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Application.DTOs.FavoritePosts;
using Optern.Application.DTOs.Post;
using Optern.Infrastructure.Response;

namespace Optern.Application.Interfaces.IFavoritePostsService
{
    public interface IFavoritePostsService
    {
        public Task<Response<IEnumerable<PostDTO>>> GetFavoritePostsByUserAsync(string userId, int lastIdx = 0, int limit = 10);
        public Task<Response<string>> AddToFavoriteAsync(AddToFavoriteDTO addToFavoriteDTO);

        public Task<Response<string>> DeleteFavoritePostAsync(string userId, int? postId = null);



    }
}
