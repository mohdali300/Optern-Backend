using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.User
{
    public class EditProfileDTO
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? JobTitle { get; set; }
        public string? AboutMe { get; set; }
    }
}
