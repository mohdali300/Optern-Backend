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
        public async Task<Response<List<PostDTO>>> GetLatestPostsAsync([Service] IPostService _postService,int count)
           => await _postService.GetLatestPostsAsync(count);

        [GraphQLDescription("Get Post Details By Id")]
        public async Task<Response<PostWithDetailsDTO>> GetPostDetailsByIdAsync([Service] IPostService _postService, int id)
           => await _postService.GetPostDetailsByIdAsync(id);

       

    }
}
