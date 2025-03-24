namespace Optern.Infrastructure.Services.RoomTracksService
{
    public class RoomTrackService(IUnitOfWork unitOfWork) : IRoomTrackService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Response<bool>> AddRoomTrack(string roomID, IEnumerable<int> data)
        {
            if (string.IsNullOrEmpty(roomID) || !data.Any())
            {
                return Response<bool>.Failure(false, "Invalid Data Model", 400);
            }
            try
            {
                if(!await _unitOfWork.Rooms.AnyAsync(r=>r.Id== roomID))
                {
                    return Response<bool>.Failure(false, "This room is not found", 404);
                }

                var roomTracks = data.Select(roomTrack => new RoomTrack
                {
                    TrackId = roomTrack,
                    RoomId = roomID,
                }).ToList();
                await _unitOfWork.RoomTracks.AddRangeAsync(roomTracks);
                await _unitOfWork.SaveAsync();
                return Response<bool>.Success(true, "RoomTracks Added Successfully", 201);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"There is a server error. Please try again later.{ex.Message}", 500);
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