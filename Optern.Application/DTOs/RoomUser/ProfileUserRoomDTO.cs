namespace Optern.Application.DTOs.RoomUser
{
    public class ProfileUserRoomDTO
    {
        public ProfileUserRoomDTO()
        {
            RoomId=string.Empty;
            Name=string.Empty;
            Description=string.Empty;
            Type=RoomType.Private;
        }

        public string? RoomId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public RoomType Type { get; set; }
    }
}
