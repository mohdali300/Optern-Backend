using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalInterfaces.IExternalAuth.IGoogleAuthService
{
    public interface IGoogleAuthService
    {
        
        Task<string> ExchangeCodeForTokenAsync(string code); 
        Task<Response<LogInResponseDTO>> GoogleLogin(string googleToken);

    }
}
