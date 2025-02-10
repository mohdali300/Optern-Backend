

namespace Optern.Application.DTOs.Task
{
    public class AttachmentDTO
    {
        public string Url { get; set; } = string.Empty;
        public AssignedUserDTO Uploader { get; set; } = new();
        public DateTime AttachmentDate { get; set; }

    }
}
