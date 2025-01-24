using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Comment
{
    public class CommentDTO
    {
       
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; } 
    }
}
