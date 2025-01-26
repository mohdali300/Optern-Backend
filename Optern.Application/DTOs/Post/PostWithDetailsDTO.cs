using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.React;
using Optern.Application.DTOs.Tags;
using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Post
{
    public class PostWithDetailsDTO
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public ContentType? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UserName { get; set; }
        public List<CommentDTO>? Comments { get; set; } = new();
        public List<ReactDTO>? Reacts { get; set; } = new();
        public List<TagDTO>? Tags { get; set; } = new();

    }
}
