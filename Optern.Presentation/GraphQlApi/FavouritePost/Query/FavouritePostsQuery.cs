using Optern.Application.DTOs.FavoritePosts;
using Optern.Application.DTOs.Post;
using Optern.Application.Interfaces.IFavoritePostsService;
using Optern.Application.Interfaces.IPostService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.FavouritePost.Query
{
    [ExtendObjectType("Query")]
    public class FavouritePostsQuery
    {
        [GraphQLDescription("Get favorite posts for a specific user")]
        public async Task<Response<IEnumerable<FavouritePostsDTO>>> GetFavoritePostsByUserAsync(
        [Service] IFavoritePostsService favoritePosts,
        string userId)
        {
            return await favoritePosts.GetFavoritePostsByUserAsync(userId);
        }
    }
}
