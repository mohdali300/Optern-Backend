

namespace Optern.Application.Interfaces.IFavoritePostsService
{
    public interface IFavoritePostsService
    {
        public Task<Response<IEnumerable<PostDTO>>> GetFavoritePostsByUserAsync(string userId, int lastIdx = 0, int limit = 10);
        public Task<Response<string>> AddToFavoriteAsync(AddToFavoriteDTO addToFavoriteDTO);

        public Task<Response<string>> DeleteFavoritePostAsync(string userId, int? postId = null);



    }
}
