using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Post
{
    public class PostDTO
    {
        public string? Title { get; set; }
        public string Content { get; set; }
        public ContentType ContentType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UserName { get; set; }
    }
}
