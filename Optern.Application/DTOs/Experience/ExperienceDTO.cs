using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.ExperienceDTO
{
    public class ExperienceDTO
    {
        public int? Id { get; set; }
        public string? JobTitle { get; set; }
        public string? Company { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsCurrentlyWork { get; set; }
        public string? JobDescription { get; set; }
        public string? Location { get; set; }
    }
}
