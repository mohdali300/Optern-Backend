

namespace Optern.Application.DTOs.Sprint
{
	public class SprintResponseDTO
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Goal { get; set; }
		public int? WorkSpaceId { get; set; }
		public int? numberOfTasks { get; set; }
		public int? finishedTasks { get; set; } 

		public SprintResponseDTO()
		{
			Id = 0;
			Title = string.Empty;
			Goal = string.Empty;
			StartDate = DateTime.MinValue;
			EndDate = DateTime.MinValue;
			WorkSpaceId = 0;
			numberOfTasks = 0;
			finishedTasks = 0;
		}

	}
}
