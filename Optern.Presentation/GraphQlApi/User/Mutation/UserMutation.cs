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
        public async Task<Response<bool>> EditProfilePicture([Service] IUserService _userService, [ID] string id,
            IFile coverPicture) =>
            await _userService.EditProfilePicture(id, coverPicture);

        public async Task<Response<bool>> AddSocialLinks([Service] IUserService _userService, string userId,
            Dictionary<string, string> links) =>
            await _userService.AddSocialLinks(userId, links);

        public async Task<Response<bool>> DeleteSocialLink([Service] IUserService _userService, string userId,
            List<string> linksKeys) =>
            await _userService.DeleteSocialLink(userId, linksKeys);
    }
}
