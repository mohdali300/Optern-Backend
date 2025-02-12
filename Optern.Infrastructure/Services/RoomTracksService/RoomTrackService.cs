

namespace Optern.Infrastructure.Services.RoomTracksService
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
                return Response<bool>.Failure(false,$"There is a server error. Please try again later.{ex.Message}", 500);

            }
        }

        public async Task<Response<bool>> DeleteRoomTrack(string roomID, int trackId)
        {
            if (string.IsNullOrEmpty(roomID) || trackId == 0)
            {
                return Response<bool>.Failure(false, "Invalid Data Model", 400);
            }

            try
            {
                var roomTrack = await _unitOfWork.RoomTracks
                    .GetByExpressionAsync(rt => rt.RoomId == roomID && rt.TrackId == trackId);

                if (roomTrack == null)
                {
                    return Response<bool>.Failure(false, "Room Track Not Found", 404);
                }

                await _unitOfWork.RoomTracks.DeleteAsync(roomTrack);
                await _unitOfWork.SaveAsync();

                return Response<bool>.Success(true, "Room Track Deleted Successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }

    }
}
