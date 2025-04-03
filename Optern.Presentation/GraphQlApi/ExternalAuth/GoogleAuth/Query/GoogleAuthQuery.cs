
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Optern.Infrastructure.ExternalDTOs.GoogleAuth;
using Optern.Infrastructure.ExternalInterfaces.IExternalAuth.IGoogleAuthService;

namespace Optern.Presentation.GraphQlApi.ExternalAuth.GoogleAuth.Query
{

    [ExtendObjectType("Query")]
    public class GoogleAuthQuery
    {
        private readonly IConfiguration _configuration;
        private readonly IGoogleAuthService _googleAuthService;
        public GoogleAuthQuery(IConfiguration configuration, IJWTService jwtService, IGoogleAuthService googleAuthService)
        {
            _configuration=configuration;
            _googleAuthService=googleAuthService;
        }

        [GraphQLName("googleCallbackAsync")]
        public async Task<Response<LogInResponseDTO>> GoogleCallbackAsync(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(), "invalid Google Code", 400);
                }
                string token = await _googleAuthService.ExchangeCodeForTokenAsync(code);
                if (token == null)
                {
                    return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(), "invalid Token", 400);
                }

                var jwt = await _googleAuthService.GoogleLogin(token);
                if (jwt == null)
                {
                    return Response<LogInResponseDTO>.Failure(new LogInResponseDTO(),"invalid Token", 400);
                }


                return Response<LogInResponseDTO>.Success(jwt.Data, "Valid User Token", 200);
            }
            catch (Exception ex)
            {
                return Response<LogInResponseDTO>.Failure($"There is a server error. Please try again later {ex.Message}", 500);
            }
        }




    }
}
