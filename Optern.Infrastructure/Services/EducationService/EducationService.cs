using Optern.Application.DTOs.Education;

namespace Optern.Infrastructure.Services.EducationService
{
    public class EducationService(IUnitOfWork unitOfWork,OpternDbContext context, IMapper mapper):IEducationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        #region AddEducation
        public async Task<Response<EducationDTO>> AddEducationAsync(string userId, EducationInputDTO model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<EducationDTO>.Failure(new EducationDTO(), "User not found!", 404);
                }
                if (string.IsNullOrEmpty(model.School))
                {
                    return Response<EducationDTO>.Failure(new EducationDTO(), "School cannot be empty!", 400);
                }

                var education = new Education
                {
                    UserId = userId,
                    School = model.School,
                    University = model.University,
                    Degree = model.Degree,
                    Major = model.Major,
                    StartYear = model.StartYear,
                    EndYear = model.EndYear,
                };

                await _unitOfWork.Education.AddAsync(education);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                var dto = _mapper.Map<EducationDTO>(education);
                return Response<EducationDTO>.Success(dto, "Education Added successfully.", 201);
            }
            catch (Exception ex)
            {
                return Response<EducationDTO>.Failure(new EducationDTO(), $"Server error {ex.Message}", 500);
            }
        }
        #endregion

        #region EditEducation
        public async Task<Response<EducationDTO>> EditEducationAsync(string userId, int educatioId, EducationInputDTO model)
        {
            if (string.IsNullOrEmpty(userId) || model == null)
                return Response<EducationDTO>.Failure(new EducationDTO(), "Invalid Data!", 400);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var education = await _unitOfWork.Education.GetByIdAsync(educatioId);
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (education == null)
                {
                    return Response<EducationDTO>.Failure(new EducationDTO(), "This Education not found!", 404);
                }
                if (user == null)
                {
                    return Response<EducationDTO>.Failure(new EducationDTO(), "User not found!", 404);
                }
                if (userId != education.UserId)
                {
                    return Response<EducationDTO>.Failure(new EducationDTO(), "User not authorized to edit", 403);
                }

                education.School = !string.IsNullOrEmpty(model.School) ? model.School : education.School;
                education.University = !string.IsNullOrEmpty(model.University) ? model.University : education.University;
                education.Major = !string.IsNullOrEmpty(model.Major) ? model.Major : education.Major;
                education.Degree = model.Degree;
                education.StartYear = !string.IsNullOrEmpty(model.StartYear) ? model.StartYear : education.StartYear;
                education.EndYear = !string.IsNullOrEmpty(model.EndYear) ? model.EndYear : education.EndYear;

                await _unitOfWork.Education.UpdateAsync(education);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                var dto = _mapper.Map<EducationDTO>(education);
                return Response<EducationDTO>.Success(dto, "Education Updated Successfully.", 200);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Response<EducationDTO>.Failure(new EducationDTO(), $"DB update error: {ex.Message}", 500);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<EducationDTO>.Failure(new EducationDTO(), $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region GetUserEducation
        public async Task<Response<IEnumerable<EducationDTO>>> GetUserEducationAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Response<IEnumerable<EducationDTO>>.Failure(new List<EducationDTO>(), "Invalid data!", 400);
            }

            try
            {
                var education = await _unitOfWork.Education.GetAllByExpressionAsync(e => e.UserId == userId);
                if (!education.Any())
                {
                    return Response<IEnumerable<EducationDTO>>.Success(new List<EducationDTO>(), "No education for this user yet.", 204);
                }
                var educationDtos = education.Select(e => _mapper.Map<EducationDTO>(e)).ToList();

                return Response<IEnumerable<EducationDTO>>.Success(educationDtos, "Education fetched successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<EducationDTO>>.Failure(new List<EducationDTO>(), $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region DeleteEducation
        public async Task<Response<bool>> DeleteEducationAsync(string userId, int educatioId)
        {
            if (string.IsNullOrEmpty(userId))
                return Response<bool>.Failure(false, "Invalid Data!", 400);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var education = await _unitOfWork.Education.GetByIdAsync(educatioId);
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (education == null)
                {
                    return Response<bool>.Failure(false, "This Education not found!", 404);
                }
                if (user == null)
                {
                    return Response<bool>.Failure(false, "User not found!", 404);
                }
                if (userId != education.UserId)
                {
                    return Response<bool>.Failure(false, "User not authorized to delete", 403);
                }

                await _unitOfWork.Education.DeleteAsync(education);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<bool>.Success(true, "Education deleted successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        } 
        #endregion
    } 
}
