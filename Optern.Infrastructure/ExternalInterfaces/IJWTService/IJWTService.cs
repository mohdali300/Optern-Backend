using Optern.Domain.Entities;
using Optern.Infrastructure.ExternalDTOs.LoginForJWT;
using Optern.Infrastructure.ExternalDTOs.Refresh_Token;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalInterfaces.IJWTService
{
    public interface IJWTService
    {
        public Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser User);
        public RefreshToken CreateRefreshToken();
        public Task<Response<LogInResponseDTO>> NewRefreshToken(RefreshTokenDTO model);
    }
}
