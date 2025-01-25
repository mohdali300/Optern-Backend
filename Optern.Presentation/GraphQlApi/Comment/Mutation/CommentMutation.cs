using Optern.Application.DTOs.Comment;
using Optern.Application.Interfaces.ICommentService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Comment.Mutation
{
    [ExtendObjectType("Mutation")]
    public class CommentMutation
    {
        [GraphQLDescription("Add a new comment to a post")]
        public async Task<Response<CommentDTO>> AddCommentAsync([Service] ICommentService _commentService, AddCommentInputDTO input, string userId)
            => await _commentService.AddCommentAsync(input, userId);


        [GraphQLDescription("Add a reply to an existing comment.")]
        public async Task<Response<CommentDTO>> AddReplyAsync([Service] ICommentService _commentService, AddReplyInputDTO input, string userId)
            => await _commentService.AddReplyAsync(input, userId);
    }
}
