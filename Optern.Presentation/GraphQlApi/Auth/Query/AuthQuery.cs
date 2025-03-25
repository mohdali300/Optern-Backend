
[ExtendObjectType("Query")]
public class AuthQuery
{
    [GraphQLDescription("Logout")]
    public async Task<Response<bool>> LogOut([Service] IAuthService _authService)
=> await _authService.LogOut();

}

