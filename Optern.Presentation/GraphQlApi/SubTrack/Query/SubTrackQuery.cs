using Optern.Application.DTOs.SubTrack;
using Optern.Application.Interfaces.ISubTrackService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.SubTrack.Query
{
	[ExtendObjectType("Query")]

	public class SubTrackQuery
	{
		[GraphQLDescription("Get All SubTracks For Track")]
		public async Task<Response<List<SubTrackDTO>>> GetAllByTrackId([Service] ISubTrackService _subTrackService, int trackId)
			=> await _subTrackService.GetAllByTrackId(trackId);
	}
}
