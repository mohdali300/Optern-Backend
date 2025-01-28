using Microsoft.AspNetCore.Http.HttpResults;
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

        public int Id { get; set; } 
		public string? Title { get; set; }
		public string? Content { get; set; }
		public string? CreatorName { get; set; } 
		public string? ProfilePicture { get; set; }
		public List<string>? Tags { get; set; } = new List<string>();
		public DateTime? CreatedDate { get; set; }

        public int ReactsCount { get; set; }
        public int CommentsCount { get; set; }

    }
}
