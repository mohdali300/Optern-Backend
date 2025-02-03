using Optern.Application.DTOs.Skills;
using Optern.Application.Interfaces.IRepositoryService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.RepositoryService
{
    public class RepositoryService(IUnitOfWork unitOfWork, OpternDbContext context) : IRepositoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;

        public async Task<Response<bool>> AddRepository(string roomId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room == null)
            {
                return Response<bool>.Failure(false, "Room Not Found", 400);
            }
            try
            {
                var repo = new Repository
                {
                    RoomId = roomId,
                };
                await _unitOfWork.Repository.AddAsync(repo);
                await _unitOfWork.SaveAsync();
                return Response<bool>.Success(true, "Repository Added Successfully");

            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }
    }
}
