

using Optern.Application.DTOs.User;

namespace Optern.Application.Interfaces.IUserService
{
    public interface IUserService
    {
        public Task<ApplicationUser> GetCurrentUserAsync();
        public  Task<Response<IEnumerable<UserDTO>>> GetAll();
        public Task<bool> IsUserExist(string userId);
    }
}
