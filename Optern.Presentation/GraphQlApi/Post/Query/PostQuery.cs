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

        [GraphQLDescription("Get Post By Id")]
        public async Task<Response<PostDTO>> GetPostByIdAsync([Service] IPostService _postService, int id)
           => await _postService.GetPostByIdAsync(id);

        [GraphQLDescription("Get all comments for a specific post")]
        public async Task<Response<List<CommentDTO>>> GetCommentsByPostIdAsync([Service] IPostService _postService, int postId)
           => await _postService.GetCommentsByPostIdAsync(postId);

        [GraphQLDescription("Get all reacts for a specific post")]
        public async Task<Response<List<ReactDTO>>> GetReactsByPostIdAsync([Service] IPostService _postService, int postId)
           => await _postService.GetReactsByPostIdAsync(postId);

    }
}
