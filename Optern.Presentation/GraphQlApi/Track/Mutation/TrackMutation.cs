using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.ITrackService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Track.Mutation
{
    public class TrackMutation
    {
        [GraphQLDescription("Add Track")]
        public async Task<Response<TrackDTO>> Add([Service] ITrackService _trackService,string name)
            => await _trackService.Add(name);
    }
}
