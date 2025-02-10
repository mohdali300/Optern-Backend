

namespace Optern.Application.Interfaces.IPostService
{
    public interface IPostService
    {
        public Task<Response<IEnumerable<PostDTO>>> GetLatestPostsAsync(string? userId = null, int lastIdx = 0, int limit = 10);

        public Task<Response<PostWithDetailsDTO>> GetPostByIdAsync(int postId,string? userId = null);

        public Task<Response<IEnumerable<PostDTO>>> GetRecommendedPostsAsync(int topN);


        public Task<Response<IEnumerable<SearchPostDTO>>> SearchPostsAsync(
            string? tagName = null,
            string? username = null,
            string? keyword = null,
            int lastIdx = 0, int limit = 10);


        public Task<Response<PostDTO>> CreatePostAsync(string userId, ManagePostDTO model);

        public Task<Response<string>> DeletePostAsync(int postId, string userId);

        public Task<Response<PostDTO>> EditPostAsync(int postId, string userId, ManagePostDTO model);







    }
}
