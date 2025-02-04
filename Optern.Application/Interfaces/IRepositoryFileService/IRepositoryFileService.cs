using Optern.Application.DTOs.RepositoryFile;
using Optern.Domain.Enums;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IRepositoryFileService
{
    public interface IRepositoryFileService
    {
        public Task<Response<RepositoryFileResponseDTO>> UploadFile(RepositoryFileDTO model, IFile file);
        public Task<Response<bool>> DeleteRepositoryFile(int repositoryFileId);
        public Task<Response<IEnumerable<RepositoryFileResponseDTO>>> GetUploadedFiles(int repositoryId, RepositoryFileSortType? sortType);
    }
}
