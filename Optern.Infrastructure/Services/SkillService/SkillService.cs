

using Optern.Domain.Entities;

namespace Optern.Infrastructure.Services.SkillService
{
    public class SkillService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : ISkillService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;



        public async Task<Response<List<string>>> GetSkillSuggestions(string word)
        {


            return Response<List<string>>.Failure($"There is a server error. Please try again later.", 500);

        }

        public async Task<Response<IEnumerable<SkillDTO>>> AddSkills(IEnumerable<SkillDTO> models)
        {
            if (models == null || !models.Any())
            {
                return Response<IEnumerable<SkillDTO>>.Failure(new List<SkillDTO>(), "Invalid Data Model", 400);
            }

            try
            {
                var skills = await ManageSkillCreation(models);
                var validationErrors = new List<string>();

                foreach (var skill in skills)
                {
                    var validate = new SkillsValidator().Validate(skill);
                    if (!validate.IsValid)
                    {
                        validationErrors.AddRange(validate.Errors.Select(e => e.ErrorMessage));
                    }
                }

                if (validationErrors.Any())
                {
                    return Response<IEnumerable<SkillDTO>>.Failure(
                        $"Invalid Data Model: {string.Join(", ", validationErrors)}", 400);
                }

                await _unitOfWork.Skills.AddRangeAsync(skills);
                await _unitOfWork.SaveAsync();

                var skillDTOs = _mapper.Map<IEnumerable<SkillDTO>>(skills);
                return Response<IEnumerable<SkillDTO>>.Success(skillDTOs, "Skills Added Successfully", 201);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<SkillDTO>>.Failure(
                    $"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }

       

        public async Task<Response<bool>> DeleteSkill(int skillId)
        {
            try
            {
                var skill = await _unitOfWork.Skills.GetByIdAsync(skillId);
                if (skill == null)
                {
                    return Response<bool>.Failure(false, "Skill not Found", 404);
                }
                await _unitOfWork.Skills.DeleteAsync(skill);
                await _unitOfWork.SaveAsync();
                return Response<bool>.Success(true, "Skill Deleted Successfully", 200);

            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false,$"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }

        #region Helper Functions
        private async Task<IEnumerable<Skills>> ManageSkillCreation(IEnumerable<SkillDTO> data)
        {
            var existingSkills = await _context.Skills
                .Select(s => new SkillDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                })
                .ToListAsync();

            var newSkills = data
                .Where(skill => !existingSkills.Any(es => es.Name.ToLower() == skill.Name.ToLower()))
                .Select(skill => new SkillDTO
                {
                    Name = skill.Name,
                })
                .ToList();

            if (newSkills.Any())
            {
                var skills = newSkills.Select(s => new Skills() { Id = s.Id, Name = s.Name });
                return skills;
            }

            return new List<Skills>();
        } 
        #endregion

    }
}
