namespace Optern.Infrastructure.ExternalServices.JWTService
{
	public class JWTService : IJWTService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public JWTService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, IHttpContextAccessor _httpContextAccessor,IConfiguration _configuration)
		{
			this._userManager = _userManager;
			this._roleManager = _roleManager;
			this._configuration = _configuration;
			this._httpContextAccessor = _httpContextAccessor;
		}
		public async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser User)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, User.UserName),
				new Claim(ClaimTypes.NameIdentifier, User.Id),
				new Claim(JwtRegisteredClaimNames.Sub, User.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Email,User.Email),
				new Claim("uid", User.Id)
			};
			var roles = await _userManager.GetRolesAsync(User);
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			SecurityKey Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
			SigningCredentials signingCred = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
			var Token = new JwtSecurityToken(
				issuer: _configuration["JWT:issuer"],
				audience: _configuration["JWT:audience"],
				claims: claims,
				signingCredentials: signingCred,
				expires: DateTime.UtcNow.AddDays(1)
				);
			return Token;
		}
		public RefreshToken CreateRefreshToken()
		{
			var randomNumber = new byte[32];
			RandomNumberGenerator.Fill(randomNumber);

			return new RefreshToken
			{
				Token = Convert.ToBase64String(randomNumber),
				ExpiresOn = DateTime.UtcNow.AddDays(10),
				CreatedOn = DateTime.UtcNow
			};
		}

		public async Task<Response<LogInResponseDTO>> NewRefreshToken(RefreshTokenDTO refresh)
		{
			var RefreshToken = refresh.RefreshToken;
			try
			{
				var user = await _userManager.Users
					.Include(u=>u.Position)
					.Include(u=>u.UserSkills).ThenInclude(us=>us.Skill)
					.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == RefreshToken));
				if (user == null)
				{
					return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(), "Not Authenticated User", 400);
				}

				var refreshToken = user.RefreshTokens.Single(t => t.Token == RefreshToken);

				if (!refreshToken.IsActive)
				{
					return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(), "InActive Token", 400);
				}

				// refreshToken.RevokedOn = DateTime.UtcNow;

				// var newRefreshToken = CreateRefreshToken();
				// user.RefreshTokens.Add(newRefreshToken);
				await _userManager.UpdateAsync(user);
				var Roles = await _userManager.GetRolesAsync(user);
				var jwtToken = await GenerateJwtToken(user);


				return Response<LogInResponseDTO>.Success(new LogInResponseDTO
				{
					UserId = user.Id,
					Name = $"{user.FirstName ?? string.Empty} {user.LastName ?? string.Empty}",
					IsAuthenticated = true,
					Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
					ProfilePicture = user.ProfilePicture,
					Roles = Roles?.ToList() ?? new List<string>(),
					Position = user.Position != null
							? new PositionDTO { Id = user.Position.Id, Name = user.Position.Name }
							: new PositionDTO(),
					Skills = user.UserSkills?.Select(us => new SkillDTO
					{
						Id = us.Skill.Id,
						Name = us.Skill.Name
					}).ToList()
				}, "Active Token", 200);

		   }
			catch (Exception ex)
			{
				return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(), "There is a server error. Please try again later.", 500);
			}
		}
	}
}
