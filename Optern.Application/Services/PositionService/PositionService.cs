

namespace Optern.Application.Services.PositionService
{
    public class PositionService :IPositionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OpternDbContext _context;

        public PositionService(IUnitOfWork unitOfWork, OpternDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region Add
        public async Task<Response<PositionDTO>> AddAsync(string name)
        {
            try
            {
                var position = new Position
                {
                    Name = name,
                };

                var validate = new PositionValidator().Validate(position);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<PositionDTO>.Failure(new PositionDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.Positions.AddAsync(position);
                return Response<PositionDTO>.Success(new PositionDTO { Id = position.Id, Name = position.Name }, "Position added successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<PositionDTO>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }
        #endregion

        #region GetAll
        public async Task<Response<List<PositionDTO>>> GetAllAsync()
        {
            try
            {
                var positions = await _unitOfWork.Positions.GetAllAsync();

                if (positions.Any())
                {
                    var positionDtos = positions.Select(s => new PositionDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                    }).ToList();

                    return Response<List<PositionDTO>>.Success(positionDtos);
                }

                return Response<List<PositionDTO>>.Success(new List<PositionDTO>(),"No positions found!", 204);
            }
            catch (Exception ex)
            {
                return Response<List<PositionDTO>>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        } 
        #endregion
    }
}
