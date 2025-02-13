
namespace Optern.Infrastructure.Services.WorkSpaceService
{
	public class WorkSpaceService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IWorkSpaceService
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly OpternDbContext _context = context;
		private readonly IMapper _mapper = mapper;



		#region Create New WorkSpace
		public async Task<Response<WorkSpaceDTO>> CreateWorkSpace(WorkSpaceDTO model)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var room = await _unitOfWork.Rooms.GetByIdAsync(model.RoomId);
				if (room == null)
				{
					return Response<WorkSpaceDTO>.Failure(new WorkSpaceDTO(), "Room Not Found!", 404);
				}
				var workSpace = new WorkSpace
				{
					Title = model.Title,
					CreatedDate = DateTime.UtcNow,
					RoomId = model.RoomId,
				};
				var validate = await new WorkSpaceValidator().ValidateAsync(workSpace);
				if (!validate.IsValid)
				{
					var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
					return Response<WorkSpaceDTO>.Failure(new WorkSpaceDTO(), $"Invalid Data Model: {errorMessages}", 400);
				}

				await _unitOfWork.WorkSpace.AddAsync(workSpace);
				await _unitOfWork.SaveAsync();
				await transaction.CommitAsync();
				var workSpaceDTO = _mapper.Map<WorkSpaceDTO>(workSpace);
				return Response<WorkSpaceDTO>.Success(workSpaceDTO, "Workspace Created Successfully", 201);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return Response<WorkSpaceDTO>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
			}
		}
		#endregion
		
		  #region get all WorkSpace
		public async Task<Response<List<WorkSpace>>> GetAllWorkSpace(string roomId)
		{
			try
			{
				var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
				if (room == null)
				{
					return Response<List<WorkSpace>>.Failure(new List<WorkSpace>(), "Room Not Found!", 404);
				}
				var workSpaces = await _context.WorkSpaces.Where(ws=>ws.RoomId == roomId).ToListAsync();

				return Response<List<WorkSpace>>.Success(workSpaces, "Workspace Fetched Successfully", 201);
			}
			catch (Exception ex)
			{
				return Response<List<WorkSpace>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
			}
		}
		#endregion

		#region Update WorkSpace
		public async Task<Response<WorkSpaceDTO>> UpdateWorkSpace(int id, string title)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				if (string.IsNullOrEmpty(title) || title.Length < 5)
				{
					return Response<WorkSpaceDTO>.Failure(new WorkSpaceDTO(), "Title field is required and must be more than 5 characters to update!", 400);
				}

				var workSpace = await _unitOfWork.WorkSpace.GetByIdAsync(id);
				if (workSpace == null)
				{
					return Response<WorkSpaceDTO>.Failure(new WorkSpaceDTO(), "WorkSpace Not Found!", 404);
				}
				workSpace.Title = title??workSpace.Title;
				await _unitOfWork.WorkSpace.UpdateAsync(workSpace);
				await _unitOfWork.SaveAsync();

				await transaction.CommitAsync();
				var workSpaceDTO = _mapper.Map<WorkSpaceDTO>(workSpace);
				return Response<WorkSpaceDTO>.Success(workSpaceDTO, "Workspace Updated Successfully", 200);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return Response<WorkSpaceDTO>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
			}
		} 
		#endregion

		#region Delete WorkSpace
		public async Task<Response<bool>> DeleteWorkSpace(int id)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var workSpace = await _unitOfWork.WorkSpace.GetByIdAsync(id);
				if (workSpace == null)
				{
					return Response<bool>.Failure(false, "WorkSpace Not Found!", 404);
				}
				await _unitOfWork.WorkSpace.DeleteAsync(workSpace);
				await _unitOfWork.SaveAsync();
				await transaction.CommitAsync();
				return Response<bool>.Success(true, "Workspace Deleted Successfully", 200);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return Response<bool>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
			}
		} 
		#endregion


	}
}
