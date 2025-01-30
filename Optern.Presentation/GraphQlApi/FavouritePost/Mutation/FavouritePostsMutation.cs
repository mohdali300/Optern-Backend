

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