

namespace Optern.Application.DTOs.Post
{
	public class PostDTO
	{

        public int Id { get; set; } 
		public string? Title { get; set; }
		public string? Content { get; set; }
		public string? CreatorName { get; set; } 
		public string? ProfilePicture { get; set; }

		public string? UserId {get;set;}
		public List<string>? Tags { get; set; } = new List<string>();
		public DateTime? CreatedDate { get; set; }

        public DateTime? EditedDate { get; set; }


        public int ReactsCount { get; set; }
        public int CommentsCount { get; set; }

    }
}
