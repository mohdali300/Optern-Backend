

using Optern.Application.DTOs.User;
using System.Collections.Generic;

namespace Optern.Infrastructure.Services.UserService
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

        public async Task<Response<IEnumerable<UserDTO>>> GetAll()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();

                if (users == null || !users.Any())
                {
                    return Response<IEnumerable<UserDTO>>.Failure(Enumerable.Empty<UserDTO>(), "No users found", 404);
                }

                return Response<IEnumerable<UserDTO>>.Success(
                    users.Select(u => new UserDTO
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        UserName = u.UserName
                    }),
                    "Users fetched successfully",
                    200
                );
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<UserDTO>>.Failure($"Server error: {ex.Message}", 500);
            }
        }

    }
}
