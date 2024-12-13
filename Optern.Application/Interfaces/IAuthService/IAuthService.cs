using Optern.Application.DTOS.Register;
using Optern.Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IAuthService
{
    public interface IAuthService
    {
        public Task<Response<string>> RegisterAsync(RegisterDTO model);
        public Task<Response<bool>> ConfirmAccount(string email, string otpCode);
    }
}
