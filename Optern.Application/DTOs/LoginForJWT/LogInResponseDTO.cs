namespace Optern.Application.DTOs.LoginForJWT
{
	public class LogInResponseDTO
	{
		public string UserId { get; set; }
		public string Name { get; set; }
		public bool IsAuthenticated { get; set; }
		public string Token { get; set; }
        public string RefreshToken { get; set; }
        public List<string> Roles { get; set; }
		
		public string? ProfilePicture { get; set; }
		public PositionDTO? Position { get; set; }
		public List<SkillDTO>? Skills { get; set; }

		public LogInResponseDTO()
		{
			IsAuthenticated = false;
			Name = string.Empty;
			UserId = string.Empty;
			Roles = new List<string>();
			Token = string.Empty;
			ProfilePicture = string.Empty;
			Position = new PositionDTO();
			Skills = new List<SkillDTO>();
		}
	}
}
