namespace Optern.Infrastructure.ExternalInterfaces.IJWTService
{
    public interface IJWTService
    {
        public Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser User);
        public RefreshToken CreateRefreshToken();
        public Task<Response<LogInResponseDTO>> NewRefreshToken(RefreshTokenDTO refreshToken);
    }
}
