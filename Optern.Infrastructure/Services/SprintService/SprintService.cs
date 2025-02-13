
namespace Optern.Infrastructure.Services.SprintService
{
    public class SprintService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper, ICacheService cacheService) : ISprintService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ICacheService _cacheService = cacheService;

        public async Task<Response<IEnumerable<SprintResponseDTO>>> GetWorkSpaceSprints(int workSpaceId)
        {
            try
            {
                var sprints = await _unitOfWork.Sprints.GetAllByExpressionAsync(s => s.WorkSpaceId == workSpaceId);
                if (sprints == null || !sprints.Any())
                {
                    return Response<IEnumerable<SprintResponseDTO>>.Failure("No Sprints Found", 404);
                }
                var orderSprints = sprints    // order by Sprints That Finished First
                   .OrderBy(s => s.EndDate)
                    .ThenBy(s => s.StartDate)
                    .ToList();
                var sprintsDTO = _mapper.Map<IEnumerable<SprintResponseDTO>>(orderSprints);
                return Response<IEnumerable<SprintResponseDTO>>.Success(sprintsDTO, "WorkSpace Fetched Successfully!", 200);
            }
            catch (Exception ex)
            {

                return Response<IEnumerable<SprintResponseDTO>>.Failure($"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }
        public async Task<Response<SprintResponseDTO>> AddSprint(AddSprintDTO model)
        {
            if (model == null)
            {
                return Response<SprintResponseDTO>.Failure("Invalid Model Data", 400);
            }

            var workSpace = await _unitOfWork.WorkSpace.GetByIdAsync(model.WorkSpaceId);
            if (workSpace == null)
            {
                return Response<SprintResponseDTO>.Failure("Workspace not Found!", 404);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sprint = new Sprint
                {
                    Title = model.Title,
                    Goal = model.Goal,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    WorkSpaceId = model.WorkSpaceId,
                };

                var validate = new SprintValidator().Validate(sprint);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<SprintResponseDTO>.Failure($"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.Sprints.AddAsync(sprint);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                var sprintDTO = _mapper.Map<SprintResponseDTO>(sprint);
                return Response<SprintResponseDTO>.Success(sprintDTO, "Sprint Added Successfully", 201);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<SprintResponseDTO>.Failure($"Server error. Please try again later. {ex.Message}", 500);
            }
        }


        public async Task<Response<SprintResponseDTO>> EditSprint(int id, EditSprintDTO model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sprint = await _unitOfWork.Sprints.GetByIdAsync(id);
                if (sprint == null)
                {
                    return Response<SprintResponseDTO>.Failure("Sprint not Found!", 404);
                }
                sprint.Title = model.Title ?? sprint.Title;
                sprint.Goal = model.Goal ?? sprint.Goal;
                sprint.StartDate = model.StartDate ?? sprint.StartDate;
                sprint.EndDate = model.EndDate ?? sprint.EndDate;

                var validate = new SprintValidator().Validate(sprint);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<SprintResponseDTO>.Failure($"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.Sprints.UpdateAsync(sprint);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                var sprintDTO = _mapper.Map<SprintResponseDTO>(sprint);
                return Response<SprintResponseDTO>.Success(sprintDTO, "Sprint Updated Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<SprintResponseDTO>.Failure($"Server error. Please try again later. {ex.Message}", 500);
            }
        }

        public async Task<Response<bool>> DeleteSprint(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sprint = await _unitOfWork.Sprints.GetByIdAsync(id);
                if (sprint == null)
                {
                    return Response<bool>.Failure(false, "Sprint not Found !", 404);
                }
                await _unitOfWork.Sprints.DeleteAsync(sprint);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return Response<bool>.Success(true, "Sprint Deleted Successfully", 200);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }

        }

        #region Store recent opened sprints in cache
        public async Task<bool> SetRecentOpenedSprintAsync(string userId, string roomId, int sprintId)
        {
            try
            {
                if (!_context.UserRooms.Where(r => r.UserId == userId && r.RoomId == roomId).Any())
                {
                    return false;
                }
                var sprint = await _unitOfWork.Sprints.GetByIdAsync(sprintId);
                if (sprint != null)
                {
                    var recent = new RecentSprintDTO
                    {
                        Id = sprintId,
                        Title = sprint.Title
                    };

                    var cacheKey = $"recent-opened-sprints:{userId},{roomId}";
                    var recentSprints = _cacheService.GetData<List<RecentSprintDTO>>(cacheKey) ?? new List<RecentSprintDTO>();

                    if (!recentSprints.Any(s => s.Id == sprintId))
                    {
                        recentSprints.Insert(0, recent);
                        if (recentSprints.Count > 10)
                        {
                            recentSprints = recentSprints.Take(10).ToList();
                        }

                        _cacheService.SetData(cacheKey, recentSprints, TimeSpan.FromDays(30));
                    }
                    else
                    {
                        var existingIndex = recentSprints.FindIndex(s => s.Id == sprintId);
                        if (existingIndex > 0)
                        {
                            recentSprints.RemoveAt(existingIndex);
                            recentSprints.Insert(0, recent);
                            _cacheService.SetData(cacheKey, recentSprints, TimeSpan.FromDays(30));
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get recent opened sprints
        public async Task<Response<IEnumerable<RecentSprintDTO>>> GetRecentOpenedSprintsAsync(string userId, string roomId)
        {
            try
            {
                if (!_context.UserRooms.Where(r => r.UserId == userId && r.RoomId == roomId).Any())
                {
                    return Response<IEnumerable<RecentSprintDTO>>.Failure(new List<RecentSprintDTO>(),"User doesn’t exist in this room, or Room id is incorrect.", 400);
                }

                var cacheKey = $"recent-opened-sprints:{userId},{roomId}";

                var recentSprints = _cacheService.GetData<List<RecentSprintDTO>>(cacheKey);
                if (recentSprints == null || !recentSprints.Any())
                {
                    recentSprints = await _context.Sprints.Include(s => s.WorkSpace)
                        .Where(s => s.WorkSpace.RoomId == roomId)
                        .OrderByDescending(s => s.StartDate).Take(5)
                        .Select(s => new RecentSprintDTO
                        {
                            Id = s.Id,
                            Title = s.Title,
                        }).ToListAsync();
                    if (recentSprints.Any())
                    {
                        _cacheService.SetData(cacheKey, recentSprints, TimeSpan.FromDays(30));

                        return Response<IEnumerable<RecentSprintDTO>>.Success(recentSprints);
                    }
                    return Response<IEnumerable<RecentSprintDTO>>.Success(new List<RecentSprintDTO>(), "There is no recent sprints", 204);
                }

                return Response<IEnumerable<RecentSprintDTO>>.Success(recentSprints);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RecentSprintDTO>>.Failure($"Server error:{ex.Message}", 500);
            }
        } 
        #endregion
    }
}
