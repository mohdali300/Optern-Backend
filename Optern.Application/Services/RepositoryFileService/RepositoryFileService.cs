using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.RepositoryFile;
using Optern.Application.DTOs.Room;
using Optern.Application.Interfaces.IRepositoryFileService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.RepositoryFileService
{
    public class RepositoryFileService(IUnitOfWork unitOfWork, OpternDbContext context, ICloudinaryService cloudinaryService, IMapper mapper) : IRepositoryFileService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly ICloudinaryService _cloudinaryService= cloudinaryService;
        private readonly IMapper _mapper= mapper;

        public async Task<Response<RepositoryFileResponseDTO>> UploadFile(RepositoryFileDTO model, IFile file)
        {
            if (model == null)
                return Response<RepositoryFileResponseDTO>.Failure(new RepositoryFileResponseDTO(), "Invalid Model Data", 400);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                string filePath = null;

                if (file != null)
                {
                    filePath = await _cloudinaryService.UploadFileAsync(file, "RoomRepositoryFiles");

                    if (string.IsNullOrEmpty(filePath))
                        return Response<RepositoryFileResponseDTO>.Failure(new RepositoryFileResponseDTO(), "File upload failed. Please try again.", 400);
                }

                var uploadedFile = new RepositoryFile
                {
                    Description = model.Description,
                    CreatedAt = DateTime.UtcNow,
                    RepositoryId = model.RepositoryId,
                    FilePath = filePath
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
        public async Task<Response<IEnumerable<RepositoryFileResponseDTO>>> GetUploadedFiles(int repositoryId)
        {
            try
            {
                var repositories = await _unitOfWork.RepositoryFile
                    .GetQueryable(r => r.RepositoryId == repositoryId, orderBy: q => q.OrderBy(r => r.CreatedAt)).ToListAsync();

                if (repositories == null || !repositories.Any())
                {
                    return Response<IEnumerable<RepositoryFileResponseDTO>>.Success(new List<RepositoryFileResponseDTO>(), "No Files Uploaded in This Repository Until Now! ", 204);
                }

                var repositoreisDTO = _mapper.Map<IEnumerable<RepositoryFileResponseDTO>>(repositories);
                return Response<IEnumerable<RepositoryFileResponseDTO>>.Success(repositoreisDTO, "Repositories Fetched Successfully ", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RepositoryFileResponseDTO>>.Failure($"An error occurred: {ex.Message}", 500);
            }
        }


    }
}
