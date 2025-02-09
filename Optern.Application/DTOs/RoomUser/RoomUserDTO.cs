

namespace Optern.Application.DTOs.RoomUser
{
    public class RoomUserDTO
    {
        public RoomUserDTO()
        {
             Id = 0;
             RoomId=string.Empty;
             UserId =string.Empty;
             UserName =string.Empty;
             ProfilePicture = string.Empty;
             IsAdmin = false;
             JoinedAt = DateTime.UtcNow;
             AcceptedAt = DateTime.UtcNow;
        }
        public int? Id { get; set; }
        public string? RoomId { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsAdmin { get; set; }=false;
        public bool IsAccepted { get; set; } = false;
        public DateTime? JoinedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public string? Role => IsAdmin ? "Leader" : "Collaborator";
    }
}
