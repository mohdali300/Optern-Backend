
using Optern.Application.Response;
namespace Optern.Infrastructure.Services.RepositoryFileService
{
    public class RepositoryFileService(IUnitOfWork unitOfWork, OpternDbContext context, ICloudinaryService cloudinaryService, IMapper mapper) : IRepositoryFileService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly ICloudinaryService _cloudinaryService= cloudinaryService;
        private readonly IMapper _mapper= mapper;

        #region Upload File
        public async Task<Response<RepositoryFileResponseDTO>> UploadFile(RepositoryFileDTO model, IFile file)
        {
            if (model == null)
                return Response<RepositoryFileResponseDTO>.Failure(new RepositoryFileResponseDTO(), "Invalid Model Data", 400);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                string filePath = null;
                string publicId = null;

                if (file != null)
                {
                    var uploadResult = await _cloudinaryService.UploadFileAsync(file, "RoomRepositoryFiles");

                    publicId = uploadResult.PublicId; // Store Public ID
                    Console.WriteLine("PUblicID", publicId);
                    filePath = uploadResult.Url; // Store File URL

                    if (string.IsNullOrEmpty(filePath))
                        return Response<RepositoryFileResponseDTO>.Failure(new RepositoryFileResponseDTO(), "File upload failed. Please try again.", 400);
                }

                var uploadedFile = new RepositoryFile
                {
                    Description = model.Description,
                    CreatedAt = DateTime.UtcNow,
                    RepositoryId = model.RepositoryId,
                    FilePath = filePath,
                    PublicId = publicId 
                };

                await _unitOfWork.RepositoryFile.AddAsync(uploadedFile);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                var fileDto = _mapper.Map<RepositoryFileResponseDTO>(uploadedFile);
                return Response<RepositoryFileResponseDTO>.Success(fileDto, "File Uploaded Successfully", 201);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<RepositoryFileResponseDTO>.Failure($"Server error. Please try again later. {ex.Message}", 500);
            }
        }
        #endregion

        #region Delete Repository
        public async Task<Response<bool>> DeleteRepositoryFile(int repositoryFileId)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var repoFile = await _unitOfWork.RepositoryFile.GetByIdAsync(repositoryFileId);
                if (repoFile == null)
                {
                    return Response<bool>.Failure(false, "Repository File not Found", 404);
                }

                await _cloudinaryService.DeleteFileAsync(repoFile.PublicId);
                await _unitOfWork.RepositoryFile.DeleteAsync(repoFile);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return Response<bool>.Success(true, "Repository File Deleted Successfully", 200);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure($"Server error. Please try again later. {ex.Message}", 500);
            }
        }
        #endregion

        #region Get Upload Files
        public async Task<Response<IEnumerable<RepositoryFileResponseDTO>>> GetUploadedFiles(int repositoryId, RepositoryFileSortType? sortType)
        {
            try
            {
                var repositoryFiles = await _unitOfWork.RepositoryFile.GetAllByExpressionAsync(r=>r.RepositoryId==repositoryId);               

                if (repositoryFiles == null || !repositoryFiles.Any())
                {
                    return Response<IEnumerable<RepositoryFileResponseDTO>>.Success(new List<RepositoryFileResponseDTO>(), "No Files Uploaded in This Repository Until Now! ", 204);
                }
                repositoryFiles = sortType switch
                {
                    RepositoryFileSortType.Name => repositoryFiles.OrderByDescending(r => r.FilePath.Split('/').Last()),
                    RepositoryFileSortType.Oldest => repositoryFiles.OrderBy(r => r.CreatedAt),
                    RepositoryFileSortType.Latest => repositoryFiles.OrderByDescending(r => r.CreatedAt),
                    _ => repositoryFiles 

                };

                var repositoreisDTO = _mapper.Map<IEnumerable<RepositoryFileResponseDTO>>(repositoryFiles);
                return Response<IEnumerable<RepositoryFileResponseDTO>>.Success(repositoreisDTO, "Repositories Fetched Successfully ", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RepositoryFileResponseDTO>>.Failure($"An error occurred: {ex.Message}", 500);
            }
        }
        #endregion

        #region Search
        public async Task<Response<IEnumerable<RepositoryFileResponseDTO>>> Search(int repositoryId,string pattern)
        {
            try
            {
                var repositoryFiles = await _context.RepositoryFiles
                    .Where(f => f.RepositoryId == repositoryId)
                    .ToListAsync();

                // publicId include FileName but may Be inclde extension of this file so we extract only  file name to return correct data for search
                var filteredFiles = repositoryFiles
                    .Where(f => Path.GetFileNameWithoutExtension(f.PublicId).ToLower().Contains(pattern.ToLower()) ||
                                f.Description.ToLower().Contains(pattern.ToLower()))
                                .ToList();
                if (!filteredFiles.Any())
                {
                    return Response<IEnumerable<RepositoryFileResponseDTO>>.Success(new List<RepositoryFileResponseDTO>(), "No Files Matches Your Search!", 204);
                }
                var repositoreisDTO = _mapper.Map<IEnumerable<RepositoryFileResponseDTO>>(filteredFiles);
                return Response<IEnumerable<RepositoryFileResponseDTO>>.Success(repositoreisDTO, "Repositories Fetched Successfully ", 200);

            }
            catch (Exception ex) { 
                            return Response<IEnumerable<RepositoryFileResponseDTO>>.Failure($"An error occurred: {ex.Message}", 500);

            }
        }
        #endregion

    }
}
