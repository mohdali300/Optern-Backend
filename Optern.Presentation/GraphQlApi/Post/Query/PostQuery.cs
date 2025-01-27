using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.React;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.IPostService;
using Optern.Application.Interfaces.ITrackService;
using Optern.Infrastructure.Response;
namespace Optern.Presentation.GraphQlApi.Post.Query
{
    [ExtendObjectType("Query")]
    public class PostQuery
    {
       

        [GraphQLDescription("Get Latest Posts")]
        public async Task<Response<IEnumerable<PostDTO>>> GetLatestPostsAsync([Service] IPostService _postService,int count)
           => await _postService.GetLatestPostsAsync(count);

        [GraphQLDescription("Get Posts Details by Id or Username")]
        public async Task<Response<IEnumerable<PostWithDetailsDTO>>> GetPostsByIdOrUserAsync(
       [Service] IPostService postService,
       int? postId = null,
       string? username = null)
        {
            return await postService.GetPostsByIdOrUserAsync(postId, username);
        }


        [GraphQLDescription("Get Recommended Posts based on reactions count")]
        public async Task<Response<IEnumerable<PostDTO>>> GetRecommendedPostsAsync([Service] IPostService postService,int topN
         ) => await postService.GetRecommendedPostsAsync(topN);


        [GraphQLDescription("Search for posts, users, or tags based on criteria")]
        public async Task<Response<IEnumerable<SearchPostDTO>>> SearchPostsAsync(
    [Service] IPostService postService,
    string? tagName = null,
    string? username = null,
    string? keyword = null
       ) => await postService.SearchPostsAsync(tagName, username, keyword);



    }
}
