using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.PTPInterview
{
   public class InterviewCachedData
    {
        public string? Code { get; set; }
        public string? Output { get; set; }
        public string? UserRole { get; set; }
        public string? Language { get; set; }
        public string? Timer { get; set; }

        public InterviewCachedData()
        {
            Code = string.Empty;
            Output = string.Empty;
            UserRole = string.Empty;
            Language = string.Empty;
            Timer = string.Empty;
        }
    }
}
