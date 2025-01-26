using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.FavoritePosts;
using Optern.Application.DTOs.Room;
using Optern.Application.Interfaces.ICommentService;
using Optern.Application.Interfaces.IFavoritePostsService;
using Optern.Application.Interfaces.IRoomService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.FavouritePost.Mutation
{
    [ExtendObjectType("Mutation")]

    public class FavouritePostsMutation
    {

        [GraphQLDescription("Add Post To Favourite")]

        public async Task<Response<string>> AddToFavoriteAsync([Service] IFavoritePostsService favoritePostsService , AddToFavoriteDTO model)
            => await favoritePostsService.AddToFavoriteAsync(model);

        [GraphQLDescription("Delete a post from favorites")]
        public async Task<Response<string>> DeleteFavoritePostAsync(
       [Service] IFavoritePostsService favoritePostsService,
       string userId,
       int? postId=null)
        {
            return await favoritePostsService.DeleteFavoritePostAsync(userId, postId);
        }
    }
}