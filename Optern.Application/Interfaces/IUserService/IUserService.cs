

namespace Optern.Application.Interfaces.IUserService
{
    public interface IUserService
    {
        public Task<ApplicationUser> GetCurrentUserAsync();
        public Task<bool> IsUserExist(string userId);
    }
}
