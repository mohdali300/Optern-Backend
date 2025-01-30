
    [ExtendObjectType("Mutation")]
    public class SubTrackMutation
    {
        [GraphQLDescription("Add SubTrack For A Track")]
        public async Task<Response<SubTrackDTO>> AddSubTrack([Service] ISubTrackService _subTrackService,string name, int trackId)
            => await _subTrackService.Add(name, trackId);
    }
