using Optern.Application.DTOs.Room;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.RoomTracksService
{
    public class RoomTrackService(IUnitOfWork unitOfWork, OpternDbContext context) : IRoomTrackService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;

        public async Task<Response<bool>> AddRoomTrack(string roomID, IEnumerable<int> data)
        {
            if (data == null || !data.Any())
            {
                return Response<bool>.Failure(false, "Invalid Data Model", 400);
            }     
            try
            {
                var roomTracks= data.Select(roomTrack=>new RoomTrack
                {
                    TrackId=roomTrack,
                    RoomId=roomID,
                }).ToList();
                await  _unitOfWork.RoomTracks.AddRangeAsync(roomTracks);
                await _unitOfWork.SaveAsync();
                return Response<bool>.Success(true, "RoomTracks Added Successfully",201);
            }
            catch (Exception ex) {
                return Response<bool>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);

            }
        }
    }
}
