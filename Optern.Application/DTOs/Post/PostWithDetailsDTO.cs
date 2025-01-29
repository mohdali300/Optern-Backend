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

		public PostWithDetailsDTO()
		{
			Title=string.Empty;
			UserName=string.Empty;
			Content=string.Empty;
			UserName= string.Empty;
			ProfilePicture=string.Empty;

		}
		public int? Id { get; set; }
		public string? Title { get; set; }
		public ContentType? ContentType { get; set; }
		public string? Content { get; set; }

		public DateTime? CreatedDate { get; set; }
		public string? UserName { get; set; }
		public string? ProfilePicture { get; set; }
		public ReactType UserReact { get; set; }

		public List<CommentDTO>? Comments { get; set; } = new();
		public List<ReactDTO>? Reacts { get; set; } = new();
		public List<TagDTO>? Tags { get; set; } = new();
		

		public int ReactCount { get; set; } 
		public int CommentCount { get; set; }



	}
}
