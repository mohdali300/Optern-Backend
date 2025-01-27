using Optern.Application.DTOs.React;
using Optern.Domain.Enums;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IReactService
{
    public interface IReactService
    {
        public Task<Response<ReactDTO>> ManageReactAsync(int postId, string userId, ReactType reactType);
        public Task<Response<CommentReactDTO>> ManageCommentReactAsync(int commentId, string userId, ReactType reactType);
        public Task<Response<List<ReactDTO>>> GetReactsAsync(int postId, ReactType? reactType = null);
        public Task<Response<List<CommentReactDTO>>> GetCommentReactsAsync(int commentId, ReactType? reactType = null);
    }
}
