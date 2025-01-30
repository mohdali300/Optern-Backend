using Optern.Application.DTOs.Sprint;
using Optern.Application.Interfaces.ISprintService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Sprint.Query
{
    [ExtendObjectType("Query")]
    public class SprintQuery
    {
        [GraphQLDescription("Get Sprints For WorkSpace")]
        public async Task<Response<IEnumerable<SprintResponseDTO>>> GetWorkSpaceSprints([Service] ISprintService _sprintService,  int workSpaceID)=>
            await _sprintService.GetWorkSpaceSprints(workSpaceID);
    }
}
