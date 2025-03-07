using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Skills
{
    public class SkillInputDTO
    {
        public string Name { get; set; }
        public SkillInputDTO()
        {
            Name = string.Empty;
        }
    }
}
