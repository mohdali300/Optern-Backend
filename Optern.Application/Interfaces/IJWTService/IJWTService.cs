using Optern.Application.DTOs.Login;
using Optern.Application.DTOs.Refresh_Token;
using Optern.Application.Response;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IJWTService
{
    public interface IJWTService
    {
        public Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser User);
        public RefreshToken CreateRefreshToken();
        public Task<Response<LogInResponseDTO>> NewRefreshToken(RefreshTokenDTO model);
    }
}
