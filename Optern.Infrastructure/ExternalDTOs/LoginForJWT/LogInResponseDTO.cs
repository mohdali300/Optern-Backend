namespace Optern.Infrastructure.ExternalDTOs.LoginForJWT
{
	public class LogInResponseDTO
	{
		public string UserId { get; set; }
		public string Name { get; set; }
		public bool IsAuthenticated { get; set; }
		public string Token { get; set; }
		public List<string> Roles { get; set; }
		
		public string? ProfilePicture { get; set; }

		public LogInResponseDTO()
		{
			IsAuthenticated = false;
			Name = string.Empty;
			UserId = string.Empty;
			Roles = new List<string>();
			Token = string.Empty;
			ProfilePicture = string.Empty;
		}
	}
}
