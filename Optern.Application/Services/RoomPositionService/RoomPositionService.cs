using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.RoomTrackService
{
    public class RoomPositionService : IRoomPositionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OpternDbContext _context;

        public RoomPositionService(IUnitOfWork unitOfWork, OpternDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region PositionRooms
        public async Task<Response<IEnumerable<CreateRoomDTO>>> GetPositionRooms(int positionId)
        {
            try
            {
                var roomDtos = await _context.RoomPositions.Include(rt => rt.Room)
                    .Where(rt => rt.PositionId == positionId)
                    .Select(rt => new CreateRoomDTO
                    {
                        Name = rt.Room.Name,
                        Description = rt.Room.Description,
                        CoverPicture = rt.Room.CoverPicture,
                        NumberOfParticipants = rt.Room.UserRooms.Count(),
                        RoomType = rt.Room.RoomType,
                        CreatedAt = rt.Room.CreatedAt,
                    })
                    .ToListAsync();

                return (roomDtos != null && roomDtos.Any()) ? Response<IEnumerable<CreateRoomDTO>>.Success(roomDtos) :
                    Response<IEnumerable<CreateRoomDTO>>.Success(new List<CreateRoomDTO>(), "No rooms found in this stack!", 204);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<CreateRoomDTO>>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        } 
        #endregion
    }
}
