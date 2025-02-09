

namespace Optern.Application.Interfaces.ITrackService
{
    public interface ITrackService
    {
        public Task<Response<List<TrackDTO>>> GetAll();
        public Task<Response<TrackDTO>> Add(string name);
    }
}
