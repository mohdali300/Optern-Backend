
    [ExtendObjectType("Mutation")]
    public class ReactMutation
    {
        [GraphQLDescription("Manage React")]
        public async Task<Response<ReactDTO>> ManageReactAsync([Service] IReactService _reactService ,int postId, string userId, ReactType reactType)
            => await _reactService.ManageReactAsync(postId, userId, reactType);

        [GraphQLDescription("Manage Comment React")]
        public async Task<Response<CommentReactDTO>> ManageCommentReactAsync([Service] IReactService _reactService, int commentId, string userId, ReactType reactType)
            => await _reactService.ManageCommentReactAsync(commentId, userId, reactType);
    }

