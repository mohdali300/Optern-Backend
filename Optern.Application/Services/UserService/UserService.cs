using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Optern.Application.Interfaces.IUserService;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
