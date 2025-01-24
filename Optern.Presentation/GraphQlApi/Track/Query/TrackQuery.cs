using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.ITrackService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Track.Query
{
	[ExtendObjectType("Query")]

	public class TrackQuery
	{
		[GraphQLDescription("Get All Tracks")]
		public async Task<Response<List<TrackDTO>>> GetAll([Service] ITrackService _trackService)
			=> await _trackService.GetAll();
	}
}
