

    [ExtendObjectType("Mutation")]
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

        [GraphQLDescription("LogIn Async")]
        public async Task<Response<LogInResponseDTO>> LogIn([Service] IAuthService _authService, LogInDTO model)
          => await _authService.LogInAsync(model);

        [GraphQLDescription("New Refresh Token")]
        public async Task<Response<LogInResponseDTO>> NewRefreshToken([Service] IJWTService _JWTService,RefreshTokenDTO refreshToken)
           =>   await  _JWTService.NewRefreshToken(refreshToken);
    }

