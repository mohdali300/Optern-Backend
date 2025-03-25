using Optern.Application.DTOs.User;
using Optern.Application.Interfaces.IUserService;

namespace Optern.Presentation.GraphQlApi.User.Query
{
    [ExtendObjectType("Query")]
    public class UserQuery
    {

        public async Task<Response<Dictionary<string, string>>> GetLinks([Service] IUserService _userService,
            string userId) =>
            await _userService.GetSocialLinks(userId);

        public async Task<Response<EditProfileDTO>> GetUserData([Service] IUserService _userService,
            string userId) =>
            await _userService.GetUserProfile(userId);


    }
}
