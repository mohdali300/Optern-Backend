using Optern.Application.DTOs.Login;
using Optern.Application.DTOs.Refresh_Token;
using Optern.Application.DTOS.Register;
using Optern.Application.Interfaces.IAuthService;
using Optern.Application.Interfaces.IJWTService;
using Optern.Application.Response;
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

        [GraphQLDescription("LogIn Async")]
        public async Task<Response<LogInResponseDTO>> LogIn([Service] IAuthService _authService,LogInDTO model)
            => await _authService.LogInAsync(model);

        [GraphQLDescription("New Refresh Token")]
        public async Task<Response<LogInResponseDTO>> NewRefreshToken([Service] IJWTService _JWTService, RefreshTokenDTO model)
           => await _JWTService.NewRefreshToken(model);
    }
}
