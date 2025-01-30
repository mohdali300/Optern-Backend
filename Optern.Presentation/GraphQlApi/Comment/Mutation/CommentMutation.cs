
    [ExtendObjectType("Mutation")]
    public class CommentMutation
    {
        [GraphQLDescription("Add a new comment to a post")]
        public async Task<Response<CommentDTO>> AddCommentAsync([Service] ICommentService _commentService, AddCommentInputDTO input, string userId)
            => await _commentService.AddCommentAsync(input, userId);


        [GraphQLDescription("Add a reply to an existing comment.")]
        public async Task<Response<CommentDTO>> AddReplyAsync([Service] ICommentService _commentService, AddReplyInputDTO input, string userId)
            => await _commentService.AddReplyAsync(input, userId);

        [GraphQLDescription("Update an existing comment.")]
        public async Task<Response<CommentDTO>> UpdateCommentAsync([Service] ICommentService _commentService, int commentId, UpdateCommentInputDTO input)
            => await _commentService.UpdateCommentAsync(commentId, input);
        [GraphQLDescription("Delete comment.")]
        public async Task<Response<bool>> DeleteCommentAsync([Service] ICommentService _commentService, int commentId)
           => await _commentService.DeleteCommentAsync(commentId);

    }
