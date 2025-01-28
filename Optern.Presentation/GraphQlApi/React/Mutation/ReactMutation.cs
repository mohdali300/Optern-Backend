using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.React;
using Optern.Application.Interfaces.ICommentService;
using Optern.Application.Interfaces.IReactService;
using Optern.Domain.Enums;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.React.Mutation
{
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
}
