

using Optern.Domain.Entities;

namespace Optern.Application.Services.UserService
{
    public class UserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork,OpternDbContext context):IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor= httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager= userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
            return await _userManager.GetUserAsync(currentUser);
        }

        public async Task<bool> IsUserExist(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user == null ? false : true;
        }
    }
}
