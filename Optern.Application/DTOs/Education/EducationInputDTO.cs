using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Education
{
    public class EducationInputDTO
    {
        public string School { get; set; }
        public string University { get; set; }
        public EducationDegree Degree { get; set; }
        public string Major { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }

        public EducationInputDTO()
        {
            School= string.Empty;
            University= string.Empty;
            Degree= new EducationDegree();
            Major= string.Empty;
            StartYear= string.Empty;
            EndYear= string.Empty;
        }
    }
}
