using Optern.Infrastructure.ExternalDTOs.Mail;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalInterfaces.IMail
{
    public interface IMailService
    {
        public Task<Response<bool>> SendEmailAsync(MailRequestDTO mailRequest);

    }
}
