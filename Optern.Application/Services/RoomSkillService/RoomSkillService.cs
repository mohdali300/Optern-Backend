using AutoMapper;
using Optern.Application.Interfaces.IRoomSkillService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.RoomSkillService
{
    public class RoomSkillService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IRoomSkillService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<bool>> AddRoomSkills(string roomID, IEnumerable<int> data)
        {
            if (data == null || !data.Any())
            {
                return Response<bool>.Failure(false, "Invalid Data Model", 400);
            }  
            try
            {
                var roomSkills = data.Select(roomSkill => new RoomSkills
                {
                    SkillId=roomSkill,
                    RoomId=roomID,  
                });
                await _unitOfWork.RoomSkills.AddRangeAsync(roomSkills);
                await _unitOfWork.SaveAsync();
                return Response<bool>.Success(true, "RoomSkills Added Successfully", 201);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
    }
}
