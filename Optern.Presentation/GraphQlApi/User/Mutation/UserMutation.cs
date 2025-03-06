using Optern.Application.DTOs.User;
using Optern.Application.Interfaces.IUserService;

namespace Optern.Presentation.GraphQlApi.User.Mutation
{
    [ExtendObjectType("Mutation")]
    public class UserMutation
    {
        public async Task<Response<bool>> EditProfile([Service] IUserService _userService, string userId,
            EditProfileDTO model) =>
            await _userService.EditProfile(userId, model);
        public async Task<Response<bool>> EditProfilePicture([Service] IUserService _userService, [ID] string userId,
            IFile picture) =>
            await _userService.EditProfilePicture(userId, picture);
    }
}
