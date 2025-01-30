
[ExtendObjectType("Mutation")]
public class PostMutation
{

    [GraphQLDescription("Create a new post")]
    public async Task<Response<PostDTO>> CreatePostAsync(
[Service] IPostService postService,
string userId,
ManagePostDTO model)
=> await postService.CreatePostAsync(userId, model);

    [GraphQLDescription("Delete Post")]
    public async Task<Response<string>> DeletePostAsync([Service] IPostService postService, int postId, string userId)
=> await postService.DeletePostAsync(postId, userId);


    [GraphQLDescription("Edit Post")]

    public async Task<Response<PostDTO>> EditPostAsync([Service] IPostService postService, int postId, string userId, ManagePostDTO model)
        => await postService.EditPostAsync(postId, userId, model);

}