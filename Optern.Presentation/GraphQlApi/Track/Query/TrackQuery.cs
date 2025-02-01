
	[ExtendObjectType("Query")]

	public class TrackQuery
	{
		[GraphQLDescription("Get All Tracks")]
		public async Task<Response<List<TrackDTO>>> GetAllTracks([Service] ITrackService _trackService)
			=> await _trackService.GetAll();

    }
