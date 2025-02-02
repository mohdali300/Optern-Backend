using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.RoomPosition;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Domain.Entities;
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
        
        public async Task<Response<bool>> AddRoomPosition(string roomID,IEnumerable<int> data)
        {
            if(data == null || !data.Any())
            {
                return Response<bool>.Failure(false, "Invalid Data Model", 400);
            }
            try
            {
                var roomPositions = data.Select(roomPosition => new RoomPosition
                {
                    PositionId = roomPosition,
                    RoomId = roomID,
                }).ToList();
                await _unitOfWork.RoomPositions.AddRangeAsync(roomPositions);
                await _unitOfWork.SaveAsync();
                return Response<bool>.Success(true, "Room Position Added Successfully",201);
            }
            catch(Exception ex)
            {
                return Response<bool>.Failure(false,$"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }

        public async Task<Response<bool>> DeleteRoomPosition(string roomID, int positionId)
        {
            if (string.IsNullOrEmpty(roomID) || positionId == 0)
            {
                return Response<bool>.Failure(false, "Invalid Data Model", 400);
            }

            try
            {
                var roomPosition = await _unitOfWork.RoomPositions
                       .GetByExpressionAsync(rp => rp.RoomId == roomID && rp.PositionId == positionId);

                if (roomPosition == null)
                {
                    return Response<bool>.Failure(false, "Room Position Not Found", 404);
                }

                await _unitOfWork.RoomPositions.DeleteAsync(roomPosition);
                await _unitOfWork.SaveAsync();

                return Response<bool>.Success(true, "Room Position Deleted Successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }

        #endregion
    }
}
