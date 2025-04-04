using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Optern.Domain.Entities;
using Optern.Infrastructure.ExternalDTOs.GoogleAuth;
using Optern.Infrastructure.ExternalInterfaces.IExternalAuth.IGoogleAuthService;

namespace Optern.Infrastructure.ExternalServices.ExternalAuth.GoogleAuthService
{
    public class GoogleAuthService : IGoogleAuthService
    {    
          private readonly IConfiguration _configuration;
          private readonly IServiceScopeFactory _serviceScopeFactory;
          private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoogleAuthService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory , IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _configuration= configuration;
            _serviceScopeFactory= serviceScopeFactory;
            _authService = authService;
            _httpContextAccessor=httpContextAccessor;
        }

        public async Task<Response<LogInResponseDTO>> GoogleLogin(string googleToken)
        {
            var payload = await VerifyGoogleToken(googleToken);
            if (payload == null)
            {
                return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(),"UnAuthorized User",401) ;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var jwtService = scope.ServiceProvider.GetRequiredService<IJWTService>();

            var user = await userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    ProfilePicture = payload.Picture,
                    EmailConfirmed=true
                };

                await userManager.CreateAsync(user);
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var jwtToken = await jwtService.GenerateJwtToken(user);
            var generatedToken= new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var userData = new LogInResponseDTO()
            {
                UserId = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                ProfilePicture = user.ProfilePicture,
                Roles = userRoles.ToList(),
                IsAuthenticated = true,
                Token = generatedToken,
            };

            var refreshToken = await _authService.GetOrCreateRefreshToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("secure_rtk", refreshToken.Token, cookieOptions);

            return  Response<LogInResponseDTO>.Success(userData,"Login Successfully",200);
        }

        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
        {

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { _configuration["GoogleAuth:ClientId"] }
            };

            return await GoogleJsonWebSignature.ValidateAsync(token, settings);
        }

        public async Task<string> ExchangeCodeForTokenAsync(string code)
        {
            using var client = new HttpClient();

            var values = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _configuration["GoogleAuth:ClientId"] },
                { "client_secret", _configuration["GoogleAuth:ClientSecret"] },
                { "redirect_uri", _configuration["GoogleAuth:redirect_uri"] },
                { "grant_type", _configuration["GoogleAuth:grant_type"] }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://oauth2.googleapis.com/token", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(responseString);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.IdToken))
            {
                return "Failed to retrieve Google access token";
            }

            return tokenResponse.IdToken;
        }


    }

}
