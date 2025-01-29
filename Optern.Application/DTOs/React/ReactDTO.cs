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
            Id = 0;
            ReactDate=DateTime.UtcNow;
            UserId=string.Empty;
            ReactType= ReactType.VOTEDOWN;
            UserName=string.Empty;
            ProfilePicture=string.Empty;
        }
        public int Id { get; set; }
        public DateTime ReactDate { get; set; }
        public string UserId { get; set; }
        public ReactType ReactType { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePicture { get; set; }
    }

    public class CommentReactDTO
    {

        public CommentReactDTO()
        {
            Id=0;
            UserId = string.Empty;
            ReactType = ReactType.NOTVOTEYET;
            UserName = string.Empty;
            ProfilePicture=string.Empty;
        }
        public int Id { get; set; }
        public ReactType ReactType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
