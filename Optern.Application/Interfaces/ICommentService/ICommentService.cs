using Optern.Application.DTOs.Comment;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.ICommentService
{
    public interface ICommentService
    {
        public Task<Response<List<CommentDTO>>> GetCommentsWithRepliesAsync(int id);
        public Task<Response<CommentDTO>> AddCommentAsync(AddCommentInputDTO input, string userId);
        public Task<Response<CommentDTO>> AddReplyAsync(AddReplyInputDTO input, string userId);
        public Task<Response<CommentDTO>> UpdateCommentAsync(int commentId, UpdateCommentInputDTO input);
        public Task<Response<bool>> DeleteCommentAsync(int commentId);

    }
}
