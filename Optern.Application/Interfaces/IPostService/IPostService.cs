using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.React;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IPostService
{
    public interface IPostService
    {
        public Task<Response<List<PostDTO>>> GetLatestPostsAsync(int count);
        public Task<Response<PostDTO>> GetPostByIdAsync(int id);
        public Task<Response<List<CommentDTO>>> GetCommentsByPostIdAsync(int postId);
        public Task<Response<List<ReactDTO>>> GetReactsByPostIdAsync(int postId);

    }
}
