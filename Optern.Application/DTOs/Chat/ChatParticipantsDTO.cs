namespace Optern.Application.DTOs.Chat
{
    public class ChatParticipantsDTO
    {
        public ChatParticipantsDTO()
        {
            Id = 0;
            UserId=string.Empty;
            Name = string.Empty;
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}
