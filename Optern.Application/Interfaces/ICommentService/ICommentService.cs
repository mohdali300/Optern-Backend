
namespace Optern.Application.Interfaces.ICommentService
{
    public interface ICommentService
    {
        public Task<Response<List<CommentDTO>>> GetRepliesForCommentAsync(int commentId);
        public Task<Response<CommentDTO>> AddCommentAsync(AddCommentInputDTO input, string userId);
        public Task<Response<CommentDTO>> AddReplyAsync(AddReplyInputDTO input, string userId);
        public Task<Response<CommentDTO>> UpdateCommentAsync(int commentId, UpdateCommentInputDTO input);
        public Task<Response<bool>> DeleteCommentAsync(int commentId);

    }
}
