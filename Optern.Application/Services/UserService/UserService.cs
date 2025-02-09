

namespace Optern.Application.Services.UserService
{
    public class UserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager):IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor= httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager= userManager;
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
            return await _userManager.GetUserAsync(currentUser);
        }
    }
}
