

namespace Optern.Application.DTOs.Sprint
{
    public class EditSprintDTO
    {
        public string? Title { get; set; }
        public string? Goal { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EditSprintDTO()
        {
            Title = string.Empty;
            Goal = string.Empty;
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MinValue;
        }



    }
}
