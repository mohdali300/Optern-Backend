namespace Optern.Infrastructure.Services.UserSkillsService
{
    public class UserSkillsService(IUnitOfWork unitOfWork,OpternDbContext context,ISkillService skillService):IUserSkillsService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly ISkillService _skillService = skillService;

        #region AddUserSkills
        public async Task<Response<bool>> AddUserSkillsAsync(string userId, List<int> skills)
        {
            try
            {
                var UserSkills = await _context.UserSkills.Where(us => us.UserId == userId)
                    .Select(us => us.SkillId).ToListAsync();
                var newUserSkills = skills.Where(skillId => !UserSkills.Contains(skillId))
                    .Select(skillId => new UserSkills
                    {
                        UserId = userId,
                        SkillId = skillId
                    });

                if (!newUserSkills.Any())
                {
                    return Response<bool>.Success(true, "User already has those skills!", 204);
                }
                await _unitOfWork.UserSkills.AddRangeAsync(newUserSkills);
                await _unitOfWork.SaveAsync();

                return Response<bool>.Success(true, "User Skills added successfully!", 201);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region ManageUserSkills
        public async Task<Response<bool>> ManageUserSkillsAsync(string userId, List<SkillInputDTO> skills)
        {
            if (string.IsNullOrEmpty(userId) || !skills.Any())
            {
                return Response<bool>.Failure(false, "Invalid User Id or Skills!", 400);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.Failure(false, "User not found!", 404);
                }

                var existingSkills = await _unitOfWork.Skills.GetAllAsync();
                var newSkills = skills.Where(s => !existingSkills.Any(ex => ex.Name.ToLower() == s.Name.ToLower()))
                    .Select(s => new SkillDTO
                    {
                        Name = s.Name
                    }).ToList();

                if (newSkills.Any())
                {
                    await _skillService.AddSkills(newSkills);
                }
                var allSkills = await _context.Skills
                    .Where(all => skills.Select(s => s.Name).Contains(all.Name))
                    .Select(s => s.Id).ToListAsync();

                var response = await AddUserSkillsAsync(userId, allSkills);
                await transaction.CommitAsync();

                return Response<bool>.Success(true, response.Message, 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region GetUserSkills
        public async Task<Response<IEnumerable<SkillDTO>>> GetUserSkillsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Response<IEnumerable<SkillDTO>>.Failure(new List<SkillDTO>(), "Invalid User Id.", 400);

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<SkillDTO>>.Failure(new List<SkillDTO>(), "User not found!", 404);
                }

                var userSkills = await _context.UserSkills.Include(us => us.Skill)
                    .Where(us => us.UserId == userId)
                    .Select(us => new SkillDTO
                    {
                        Id = us.Skill.Id,
                        Name = us.Skill.Name,
                    }).ToListAsync();

                if (!userSkills.Any())
                {
                    return Response<IEnumerable<SkillDTO>>.Success(new List<SkillDTO>(), "No skills for this user!", 204);
                }

                return Response<IEnumerable<SkillDTO>>.Failure(userSkills, "", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<SkillDTO>>.Failure(new List<SkillDTO>(), $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region DeleteUserSkills
        public async Task<Response<bool>> DeleteUserSkillsAsync(string userId, List<int> skillsIds)
        {
            if (string.IsNullOrEmpty(userId) || !skillsIds.Any())
            {
                return Response<bool>.Failure(false, "Invalid user id or skills IDs", 400);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var skills = await _context.Skills.Where(s => skillsIds.Contains(s.Id))
                    .Select(s => s.Id).ToListAsync();
                if (!skills.Any())
                {
                    return Response<bool>.Failure(false, "Skills not found.", 404);
                }

                var userskills = await _context.UserSkills.Include(us => us.Skill)
                    .Where(us => us.UserId == userId && skills.Contains(us.Skill.Id)).ToListAsync();
                if (!userskills.Any())
                {
                    return Response<bool>.Success(true, "User already doesn’t have those skills.", 204);
                }

                await _unitOfWork.UserSkills.DeleteRangeAsync(userskills);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<bool>.Success(true, "User Skills deleted successfully.", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        } 
        #endregion
    }
}
