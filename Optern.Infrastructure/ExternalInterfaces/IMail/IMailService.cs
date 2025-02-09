namespace Optern.Infrastructure.ExternalInterfaces.IMail
{
    public interface IMailService
    {
        public Task<Response<bool>> SendEmailAsync(MailRequestDTO mailRequest);

    }
}
