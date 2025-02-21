
    [ExtendObjectType("Query")]
    public class CommentQuery
    {
        [GraphQLDescription("Get Replies For Comment")]
        public async Task<Response<List<CommentDTO>>> GetRepliesForCommentAsync([Service] ICommentService _commentService,int id,string userId)
            => await _commentService.GetRepliesForCommentAsync(id,userId);   

    }

