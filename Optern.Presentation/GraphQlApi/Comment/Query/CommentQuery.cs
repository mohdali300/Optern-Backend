using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.Post;
using Optern.Application.Interfaces.ICommentService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Comment.Query
{
    [ExtendObjectType("Query")]
    public class CommentQuery
    {
        [GraphQLDescription("Get Comment with Replies")]
        public async Task<Response<List<CommentDTO>>> GetCommentsWithRepliesAsync([Service] ICommentService _commentService,int id)
            => await _commentService.GetCommentsWithRepliesAsync(id);


       

    }
}
