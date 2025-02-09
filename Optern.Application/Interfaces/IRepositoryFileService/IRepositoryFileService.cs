

namespace Optern.Application.Interfaces.IRepositoryFileService
{
    public interface IRepositoryFileService
    {
        public Task<Response<RepositoryFileResponseDTO>> UploadFile(RepositoryFileDTO model, IFile file);
        public Task<Response<bool>> DeleteRepositoryFile(int repositoryFileId);
        public Task<Response<IEnumerable<RepositoryFileResponseDTO>>> GetUploadedFiles(int repositoryId, RepositoryFileSortType? sortType);
        public Task<Response<IEnumerable<RepositoryFileResponseDTO>>> Search(int repositoryId, string pattern);
    }
}
