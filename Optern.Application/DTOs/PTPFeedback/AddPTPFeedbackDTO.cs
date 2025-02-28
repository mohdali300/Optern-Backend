
namespace Optern.Application.DTOs.PTPFeedback
{
    public class AddPTPFeedbackDTO
    {
        public string UserId { get; set; }  
        public int PTPInterviewId { get; set; }
        public List<KeyValuePair<int, string>>? Ratings { get; set; } = new();

        public  string? InterviewerPerformance { get; set; } =string.Empty;
        public string? Improvement { get; set; } = string.Empty;    
    }

}
