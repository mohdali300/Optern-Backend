

namespace Optern.Infrastructure.Services.AuthService
{

	public class AuthService : IAuthService
	{

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMailService _mailService;
		private readonly IJWTService _jWTService;
		private readonly OTP _OTP;
		public AuthService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager,
			IHttpContextAccessor _httpContextAccessor, IMailService _mailService, OTP _OTP, IJWTService _jWTService)
		{
			this._userManager = _userManager;
			this._roleManager = _roleManager;
			this._httpContextAccessor = _httpContextAccessor;
			this._mailService = _mailService;
			this._OTP = _OTP;
			this._jWTService = _jWTService; 
		}

		#region Register
		public async Task<Response<string>> RegisterAsync(RegisterDTO model)
		{
			try
			{
				if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.UserName))
				{
					return Response<string>.Failure("Invalid Data Model", 400);
				}

				var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
				var existingUserByUserName = await _userManager.FindByNameAsync(model.UserName);

				if (existingUserByEmail != null)
				{
					return Response<string>.Failure("This Email is already used before!", 400);
				}

				if (existingUserByUserName != null)
				{
					return Response<string>.Failure("This UserName is already used before!", 400);
				}

				var user = await CreateUserAsync(model);
				if (user == null)
				{
					return Response<string>.Failure("There was an error creating the user", 400);
				}

				if (!await _roleManager.RoleExistsAsync("User"))
				{
					var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
					if (!roleResult.Succeeded)
					{
						return Response<string>.Failure("Error occurred while creating role", 400);
					}
				}

				var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");
				if (!addToRoleResult.Succeeded)
				{
					return Response<string>.Failure("Error occurred while assigning role to the user", 400);
				}

				var otpResult = await _OTP.SendRegisterationOTPAsync(model.Email);

				if (!otpResult.IsSuccess)
				{
					return Response<string>.Failure("There is an Error Happen While Sending Email", 400);
				}

				return Response<string>.Success("Account Registered Successfully, Pleas Go To your Email and Confirm Your Account", "Account Registered Successfully, Pleas Go To your Email and Confirm Your Account", 200);
			}
			catch (Exception ex)
			{
				return Response<string>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
			}
		}
		#endregion

		#region Confirm Account
		public async Task<Response<bool>> ConfirmAccount(string email, string otpCode)
		{
			try
			{
				var otpData = _httpContextAccessor.HttpContext.Request.Cookies["OtpRegister"];
				if (string.IsNullOrEmpty(otpData))
				{
					return Response<bool>.Failure("Invalid or Expired OTP", 400);
				}

				var parts = otpData.Split('|');
				if (parts.Length != 3) 
				{
					return Response<bool>.Failure("Invalid OTP data", 400);
				}

				var storedOtp = parts[0];
				var storedEmail = parts[2];

				if (email != storedEmail || otpCode != storedOtp)
				{
					return Response<bool>.Failure("Invalid or Expired OTP", 400);
				}

				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
				{
					return Response<bool>.Failure("Invalid Email", 404);
				}

				user.EmailConfirmed = true;
				await _userManager.UpdateAsync(user);

				_httpContextAccessor.HttpContext.Response.Cookies.Delete("OtpRegister");

				return Response<bool>.Success(true, "Account Confirmed Successfully", 200);
			}
			catch (Exception ex)
			{
				return Response<bool>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
			}
		}

		#endregion

		#region SendResetPassword
		public async Task<Response<bool>> SendResetPasswordEmail(string email)
		{
			try
			{
				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
				{
					return Response<bool>.Failure("The Email Address doesn't Exist", 404);
				}

				var otpResult = await _OTP.SendResetPasswordOTPAsync(email);

				if (otpResult.IsSuccess)
				{
					return Response<bool>.Success(true, "OTP has been successfully sent.", 200);
				}

				return Response<bool>.Failure(otpResult.Message, 400);

			}
			catch (Exception ex)
			{
				return Response<bool>.Failure($"An unexpected error occurred: {ex.Message}", 500);
			}
		}
		#endregion

		#region VerifyOtpAndResetPassword
		public async Task<Response<bool>> VerifyOtpAndResetPassword(ResetPasswordDto dto)
		{
			try
			{
				var otpKey = "OtpResetPassword";
				if (!_OTP.IsOtpValid(otpKey))
				{
					return Response<bool>.Failure("Invalid or expired OTP", 400);
				}

				var storedOtp = _httpContextAccessor.HttpContext.Request.Cookies[otpKey];
				var parts = storedOtp.Split('|');
				var otp = parts[0];
				var expirationTime = parts[1];
				var email = parts[2];

				if (otp != dto.Otp)
				{
					return Response<bool>.Failure("Invalid OTP", 400);
				}

				if (dto.NewPassword != dto.ConfirmPassword)
				{
					return Response<bool>.Failure("Password and Confirm Password do not match.", 400);
				}

				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
				{
					return Response<bool>.Failure("User not found", 404);
				}

				var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
				var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);

				if (result.Succeeded)
				{
					_httpContextAccessor.HttpContext.Response.Cookies.Delete(otpKey);
					return Response<bool>.Success(true, "Password has been successfully reset.", 200);
				}

				return Response<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)), 500);
			}
			catch (Exception ex)
			{
				return Response<bool>.Failure($"An unexpected error occurred: {ex.Message}", 500);
			}
		}
		#endregion

		#region ResendOtp
		public async Task<Response<bool>> ResendOtpAsync(string email, OtpType otpType)
		{
			try
			{
				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
				{
					return Response<bool>.Failure("The Email Address doesn't Exist", 404);
				}
				var otpResult = await _OTP.ResendOtpAsync(email, otpType);

				if (otpResult.IsSuccess)
				{
					return Response<bool>.Success(true, "OTP has been successfully sent.", 200);
				}

				return Response<bool>.Failure(otpResult.Message, 400);
			}
			catch (Exception ex)
			{
				return Response<bool>.Failure($"Failed to resend OTP. Error: {ex.Message}");
			}
		}
		#endregion

		#region LogIn
		public async Task<Response<LogInResponseDTO>> LogInAsync(LogInDTO model)
		{
			try
			{
				if (!ValidateEmailAndPassword(model))
				{
					return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(), "Invalid Data in Model", 400);
				}

                var user = await _userManager.Users
					.Include(u => u.Position)
					.Include(u=>u.UserSkills).ThenInclude(us=>us.Skill)
					.FirstOrDefaultAsync(u => u.Email == model.Email);

				if (user == null || !user.EmailConfirmed)
				{
					return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(), "Invalid Email or Password", 404);
				}

				var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
				if (!isPasswordValid)
				{
					return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(), "Invalid Email or Password", 400);
				}

				var token = await _jWTService.GenerateJwtToken(user);
				var refreshToken = await GetOrCreateRefreshToken(user);

				var userRoles = await _userManager.GetRolesAsync(user);
				var cookieOptions = new CookieOptions
				{
					HttpOnly = true,
					Secure = false,
					SameSite = SameSiteMode.Lax,
				};

				_httpContextAccessor.HttpContext.Response.Cookies.Append("secure_rtk", refreshToken.Token, cookieOptions);
				return Response<LogInResponseDTO>.Success(
					new LogInResponseDTO
					{
						UserId = user.Id,
						Name = $"{user.FirstName ?? string.Empty} {user.LastName ?? string.Empty}",
						IsAuthenticated = true,
						Token = new JwtSecurityTokenHandler().WriteToken(token),
						Roles = userRoles?.ToList() ?? new List<string>(),
						ProfilePicture = user.ProfilePicture,
						Position = user.Position != null
							? new PositionDTO { Id = user.Position.Id, Name = user.Position.Name }
							: new PositionDTO(),
						Skills = user.UserSkills?.Select(us => new SkillDTO
						{
							Id = us.Skill.Id,
							Name = us.Skill.Name
						}).ToList()
					},
					"User login successfully", 200);
			}
			catch (Exception ex)
			{
				return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(), "There is a server error. Please try again later.", 500);
			}
		} 
		#endregion

		#region Private Helpers Functions
		private async Task<ApplicationUser> CreateUserAsync(RegisterDTO model)
		{
			var user = new ApplicationUser
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				UserName = model.UserName,
			};

			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
			{
				return null;
			}
			return user;
		}
		private bool ValidateEmailAndPassword(LogInDTO model)
		{
			return !string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password);
		}

		private async Task<RefreshToken> GetOrCreateRefreshToken(ApplicationUser user)
		{
			var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
			if (activeToken != null)
			{
				return activeToken;
			}

			var newRefreshToken = _jWTService.CreateRefreshToken();
			user.RefreshTokens.Add(newRefreshToken);
			await _userManager.UpdateAsync(user);
			return newRefreshToken;
		} 
		#endregion

	}
}
