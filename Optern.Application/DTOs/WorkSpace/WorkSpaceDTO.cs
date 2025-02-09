

namespace Optern.Application.DTOs.WorkSpace
{
    public class WorkSpaceDTO
    {
        public int? Id { get; set; }
        public string RoomId { get; set; }
        public string Title { get; set; }
        public DateTime? CreatedDate { get; set; }

        public WorkSpaceDTO() {
        
            RoomId = string.Empty;
            Id = 0;
            Title = string.Empty;   
            CreatedDate = DateTime.MinValue;
        }
    }
}
