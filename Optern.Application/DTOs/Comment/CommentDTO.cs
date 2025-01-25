using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Comment
{

    public class CommentDTO
    {
        public CommentDTO()
        {
            Id = 0;
            Content=string.Empty;
            CommentDate = DateTime.Now;
            UserName = string.Empty;
            Replies= new List<CommentDTO>();


        }
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime? CommentDate { get; set; }
        public string? UserName { get; set; }
        public List<CommentDTO>? Replies { get; set; } = new List<CommentDTO>(); // For nested replies
    }
    public class AddCommentInputDTO
    {
        public int PostId { get; set; }
        public string Content { get; set; }
    }

    public class AddReplyInputDTO
    {
        public int ParentId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
    }
}
