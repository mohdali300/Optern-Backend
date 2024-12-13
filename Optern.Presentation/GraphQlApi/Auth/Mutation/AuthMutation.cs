using Optern.Application.DTOs.ResetPassword;
using Optern.Application.DTOS.Register;
using Optern.Application.Helpers;
using Optern.Application.Interfaces.IAuthService;
using Optern.Application.Response;
using Optern.Domain.Enums;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Optern.Presentation.GraphQlApi.Auth.Mutation
{
    public class AuthMutation
    {
        [GraphQLDescription("Register")]
        public async Task<Response<string>> Register([Service] IAuthService _authService, RegisterDTO request) =>
           await _authService.RegisterAsync(request);

        [GraphQLDescription("Confirm Account")]

        public async Task<Response<bool>> ConfirmAccount([Service] IAuthService _authService,string email, string otpCode) =>
             await _authService.ConfirmAccount(email,otpCode);


        [GraphQLDescription("Send Reset Password OTP to Email")]
        public async Task<Response<bool>> SendResetPasswordEmail(
                [Service] IAuthService _authService,
                string email)
        => await _authService.SendResetPasswordEmail(email);

        [GraphQLDescription("Reset Password")]

        public async Task<Response<bool>> VerifyOtpAndResetPassword([Service] IAuthService _authService, ResetPasswordDto resetPassword)

            => await _authService.VerifyOtpAndResetPassword(resetPassword);

        [GraphQLDescription("Resend OTP for registration or password reset.")]
        public async Task<Response<bool>> ResendOtp(
       [Service] OTP otpHelper,
       string email,
       OtpType otpType)
           =>  await otpHelper.ResendOtpAsync(email, otpType);
        

    }
}
