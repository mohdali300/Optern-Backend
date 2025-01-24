using Optern.Application.DTOs.SubTrack;
using Optern.Application.Interfaces.ISubTrackService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.SubTrack.Mutation
{
    public class SubTrackMutation
    {
        [GraphQLDescription("Add SubTrack For A Track")]
        public async Task<Response<SubTrackDTO>> Add([Service] ISubTrackService _subTrackService,string name, int trackId)
            => await _subTrackService.Add(name, trackId);
    }
}
