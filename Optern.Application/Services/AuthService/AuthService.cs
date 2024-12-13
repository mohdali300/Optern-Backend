using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Optern.Application.DTOs.Mail;
using Optern.Application.DTOS.Register;
using Optern.Application.Helpers;
using Optern.Application.Interfaces.IAuthService;
using Optern.Application.Response;
using Optern.Domain.Entities;
using Optern.Infrastructure.ExternalServices.MailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.AuthService
{
    public class AuthService: IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;
        private readonly OTP _OTP;


        public AuthService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager,
            IHttpContextAccessor _httpContextAccessor, IMailService _mailService, OTP OTP)
        {
            this._userManager = _userManager;
            this._roleManager = _roleManager;
            this._httpContextAccessor = _httpContextAccessor;
            this._mailService = _mailService;
            this._OTP = OTP;
        }
        public async Task<Response<string>> RegisterAsync(RegisterDTO model)
        {
            try
            {
                if (model == null|| string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.UserName) )
                {
                    return Response<string>.Failure("Invalid Data Model", 400);
                }

                var isEmailExisted = await _userManager.FindByEmailAsync(model.Email);
                var isUserNameExisted = await _userManager.FindByNameAsync(model.UserName);

                if (isEmailExisted != null)
                {
                    return Response<string>.Failure("This Email is already used before!", 400);
                }

                if (isUserNameExisted != null)
                {
                    return Response<string>.Failure("This UserName is already used before!", 400);
                }

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
                return Response<string>.Failure("There is a server error. Please try again later.", 500);
            }
        }

        public async Task<Response<bool>> ConfirmAccount(string email, string otpCode)
        {
            try
            {
                string otpKey = "OtpRegister";
                var storedOtp = _httpContextAccessor.HttpContext.Request.Cookies[otpKey];
                if (storedOtp == null || storedOtp.Split("|")[0] != otpCode)
                {
                    return Response<bool>.Failure("Invalid OTP ", 400);
                }
                if (!_OTP.IsOtpValid(otpKey))
                {
                    return Response<bool>.Failure("Expired OTP", 400);
                }
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Response<bool>.Failure("User not Found", 404);
                }
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return Response<bool>.Success(true,"Account Confirmed Successfully", 200);

            }
            catch (Exception ex) {
                return Response<bool>.Failure("There is a server error. Please try again later.", 500);

            }

        }

    }
}
