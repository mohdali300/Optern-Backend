

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
