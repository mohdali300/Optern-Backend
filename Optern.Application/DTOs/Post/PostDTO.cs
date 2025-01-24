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
        public ContentType? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UserName { get; set; }
    }
}
