using Optern.Application.DTOs.Mail;
using Optern.Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalServices.MailService
{
    public interface IMailService
    {
        public Task<Response<bool>> SendEmailAsync(MailRequestDTO mailRequest);

    }
}
