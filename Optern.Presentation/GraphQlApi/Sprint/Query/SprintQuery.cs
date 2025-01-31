
    [ExtendObjectType("Query")]
    public class SprintQuery
    {
        [GraphQLDescription("Get Sprints For WorkSpace")]
        public async Task<Response<IEnumerable<SprintResponseDTO>>> GetWorkSpaceSprints([Service] ISprintService _sprintService,  int workSpaceID)=>
            await _sprintService.GetWorkSpaceSprints(workSpaceID);
    }

