using Optern.Application.DTOs.React;
using Optern.Application.Interfaces.IReactService;
using Optern.Domain.Enums;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.React.Query
{
    [ExtendObjectType("Query")]
    public class ReactQuery
    {
        [GraphQLDescription("Reacts on the post")]
        public async Task<Response<List<ReactDTO>>> GetReactsAsync([Service] IReactService _reactService,int postId, ReactType? reactType = null)
            => await _reactService.GetReactsAsync(postId,reactType);

        [GraphQLDescription("Reacts on Comments")]
        public async Task<Response<List<CommentReactDTO>>> GetCommentReactsAsync([Service] IReactService _reactService, int commentId, ReactType? reactType = null)
            => await _reactService.GetCommentReactsAsync(commentId, reactType);
    }
}
