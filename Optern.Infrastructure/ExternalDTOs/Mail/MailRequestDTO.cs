namespace Optern.Infrastructure.ExternalDTOs.Mail
{
    public class MailRequestDTO
    {
        public string? Email { get; set; }
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Body { get; set; }
        public IList<IFormFile>? Attachments { get; set; }
    }
}
