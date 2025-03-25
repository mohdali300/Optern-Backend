

using Optern.Application.DTOs.User;

namespace Optern.Application.Interfaces.IUserService
{
    public interface IUserService
    {
        public Task<ApplicationUser> GetCurrentUserAsync();
        public  Task<Response<IEnumerable<UserDTO>>> GetAll();
        public Task<bool> IsUserExist(string userId);
        public Task<Response<bool>> EditProfile(string userId, EditProfileDTO model);
        public Task<Response<bool>> EditProfilePicture(string userId, IFile image);
        public Task<Response<bool>> AddSocialLinks(string userId, Dictionary<string, string> links);
        public Task<Response<bool>> DeleteSocialLink(string userId, List<string> links);
        public Task<Response<Dictionary<string, string>>> GetSocialLinks(string userId);
          public Task<Response<EditProfileDTO>> GetUserProfile(string userId);
    }
}
