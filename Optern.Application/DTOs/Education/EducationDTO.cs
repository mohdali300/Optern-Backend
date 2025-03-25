using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Education
{
    public class EducationDTO
    {
        public int Id { get; set; }
        public string School { get; set; }
        public string University { get; set; }
        public string Major { get; set; } 
        public EducationDegree Degree { get; set; } 
        public string StartYear { get; set; }
        public string EndYear { get; set; }

        public EducationDTO()
        {
            Id = 0;
            School = string.Empty;
            University = string.Empty;
            Degree = EducationDegree.Associate;
            Major = string.Empty;
            StartYear = string.Empty;
            EndYear = string.Empty;
        }
    }
}
