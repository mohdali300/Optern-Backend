using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Task
{
    public class AttachmentDTO
    {
        public string Url { get; set; } = string.Empty;
        public AssignedUserDTO Uploader { get; set; } = new();

    }
}
