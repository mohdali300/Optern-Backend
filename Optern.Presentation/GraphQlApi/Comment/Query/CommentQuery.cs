
    [ExtendObjectType("Query")]
    public class CommentQuery
    {
        [GraphQLDescription("Get Replies For Comment")]
        public async Task<Response<List<CommentDTO>>> GetRepliesForCommentAsync([Service] ICommentService _commentService,int id)
            => await _commentService.GetRepliesForCommentAsync(id);   

    }

