using Optern.Application.DTOS.Register;
using Optern.Application.Interfaces.IAuthService;
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
    }
}
