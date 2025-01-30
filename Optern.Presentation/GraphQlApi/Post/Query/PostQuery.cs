
 [ExtendObjectType("Query")]
    public class PostQuery
    {
       

        [GraphQLDescription("Get Latest Posts")]
        public async Task<Response<IEnumerable<PostDTO>>> GetLatestPostsAsync([Service] IPostService _postService, string? userId = null, int lastIdx = 0, int limit = 10)
           => await _postService.GetLatestPostsAsync(userId,lastIdx,limit);

       [GraphQLDescription("Get Posts Details by Id")]
        public async Task<Response<PostWithDetailsDTO>> GetPostByIdAsync([Service] IPostService _postService,int postId,string? userId = null)
            =>await _postService.GetPostByIdAsync(postId,userId);


        [GraphQLDescription("Get Recommended Posts based on reactions count")]
        public async Task<Response<IEnumerable<PostDTO>>> GetRecommendedPostsAsync([Service] IPostService postService,int topN
         ) => await postService.GetRecommendedPostsAsync(topN);


        [GraphQLDescription("Search for posts, users, or tags based on criteria")]
        public async Task<Response<IEnumerable<SearchPostDTO>>> SearchPostsAsync(
    [Service] IPostService postService,
    string? tagName = null,
    string? username = null,
    string? keyword = null,
    int lastIdx = 0, int limit = 10) => await postService.SearchPostsAsync(tagName, username, keyword,lastIdx,limit);



    }