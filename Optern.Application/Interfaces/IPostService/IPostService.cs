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
        public Task<Response<IEnumerable<PostDTO>>> GetLatestPostsAsync(string? userId = null, int? lastIdx = null, int limit = 10);

        public Task<Response<PostWithDetailsDTO>> GetPostByIdAsync(int postId);

        public Task<Response<IEnumerable<PostDTO>>> GetRecommendedPostsAsync(int topN);


        public Task<Response<IEnumerable<SearchPostDTO>>> SearchPostsAsync(
            string? tagName = null,
            string? username = null,
            string? keyword = null);


        public Task<Response<PostDTO>> CreatePostAsync(string userId, ManagePostDTO model);

        public Task<Response<string>> DeletePostAsync(int postId, string userId);

        public Task<Response<PostDTO>> EditPostAsync(int postId, string userId, ManagePostDTO model);







    }
}
