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
        public Task<Response<IEnumerable<FavouritePostsDTO>>> GetFavoritePostsByUserAsync(string userId);
        public Task<Response<string>> AddToFavoriteAsync(AddToFavoriteDTO addToFavoriteDTO);

        public Task<Response<string>> DeleteFavoritePostAsync(string userId, int? postId = null);



    }
}
