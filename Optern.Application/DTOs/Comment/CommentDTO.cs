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

        public string TimeAgo
        {
            get
            {
                if (CommentDate == null) return "Unknown";

                var timeSpan = DateTime.UtcNow - CommentDate.Value;

                if (timeSpan.TotalMinutes < 1) return "Just now";
                if (timeSpan.TotalMinutes < 60) return $"{Math.Floor(timeSpan.TotalMinutes)} minutes ago";
                if (timeSpan.TotalHours < 24) return $"{Math.Floor(timeSpan.TotalHours)} hours ago";
                if (timeSpan.TotalDays < 7) return $"{Math.Floor(timeSpan.TotalDays)} days ago";
                if (timeSpan.TotalDays < 30) return $"{Math.Floor(timeSpan.TotalDays / 7)} weeks ago";
                if (timeSpan.TotalDays < 365) return $"{Math.Floor(timeSpan.TotalDays / 30)} months ago";
                return $"{Math.Floor(timeSpan.TotalDays / 365)} years ago";
            }
        }
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
    public class UpdateCommentInputDTO
    {
        
        public string UpdatedContent { get; set; } 
    }

}
