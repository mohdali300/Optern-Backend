namespace Optern.Infrastructure.ExternalServices.MailService
{
    public class MailService : IMailService
    {

        private readonly MailSettingsDTO _mailSettings;

        public MailService(IOptions<MailSettingsDTO> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task<Response<bool>> SendEmailAsync(MailRequestDTO mailRequest)
        {
            try
            {
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_mailSettings.Email),
                    Subject = mailRequest.Subject
                };

                email.To.Add(MailboxAddress.Parse(mailRequest.Email));

                var builder = new BodyBuilder
                {
                    HtmlBody = mailRequest.Body
                };

                if (mailRequest.Attachments != null && mailRequest.Attachments.Any())
                {
                    foreach (var attachment in mailRequest.Attachments)
                    {
                        if (attachment.Length > 0)
                        {
                            using var ms = new MemoryStream();
                            await attachment.CopyToAsync(ms);
                            builder.Attachments.Add(attachment.FileName, ms.ToArray(), MimeKit.ContentType.Parse(attachment.ContentType));
                        }
                    }
                }

                email.Body = builder.ToMessageBody();

                email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));

                using var smtp = new MailKit.Net.Smtp.SmtpClient
                {
                    CheckCertificateRevocation = false
                };

                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return Response<bool>.Success(true, "Email sent successfully.");
            }
            catch (SmtpException smtpEx)
            {
                return Response<bool>.Failure(
                    "An error occurred while sending the email. Please try again later.",
                    500,
                    new List<string> { smtpEx.Message }
                );
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(
                    "An unexpected error occurred while sending the email. Please try again later.",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
