using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.ITrackService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Track.Query
{
	[ExtendObjectType("Query")]

	public class TrackQuery
	{
		[GraphQLDescription("Get All Tracks")]
		public async Task<Response<List<TrackDTO>>> GetAllTracks([Service] ITrackService _trackService)
			=> await _trackService.GetAll();

		[GraphQLDescription("Get all tracks with their sub tracks")]
		public async Task<Response<List<TrackWithSubTracksDTO>>> GetAllTracksWithSubTracks([Service] ITrackService _trackService)
			=> await _trackService.GetAllWithSubTracks();

    }
}
