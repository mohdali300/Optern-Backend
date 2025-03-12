using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.PTPInterview
{
    public class PartnerDTO
    {
        public string Id { get; set; } 
        public string Name { get; set; } 
        public string ProfilePicture { get; set; } 

        public PartnerDTO()
        {
        Id=string.Empty;
        Name=string.Empty;
        ProfilePicture = string.Empty;
        }
    }

}
