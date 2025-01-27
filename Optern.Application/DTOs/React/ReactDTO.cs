using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.React
{
    public class ReactDTO
    {
        public ReactDTO() { 
            ReactDate=DateTime.UtcNow;
            UserId=string.Empty;
            ReactType= ReactType.Insightful;
            UserName=string.Empty;


        }
        public DateTime ReactDate { get; set; }
        public string UserId { get; set; }
        public ReactType ReactType { get; set; }
        public string? UserName { get; set; }
    }

    public class CommentReactDTO
    {
        public CommentReactDTO()
        {
            UserId = string.Empty;
            ReactType = ReactType.Insightful;
            UserName = string.Empty;
        }
        public ReactType ReactType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
