using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Skills
{
    public class SkillDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SkillDTO() {
            Id = 0;
            Name=string.Empty;
        }
    }
}
