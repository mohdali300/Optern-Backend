
	[ExtendObjectType("Query")]
	public class SubTrackQuery
	{
		[GraphQLDescription("Get All SubTracks For Track")]
		public async Task<Response<List<SubTrackDTO>>> GetAllSubTracksByTrackId([Service] ISubTrackService _subTrackService, int trackId)
			=> await _subTrackService.GetAllByTrackId(trackId);
	}

