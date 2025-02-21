
[ExtendObjectType("Query")]
public class SprintQuery
{
	[GraphQLDescription("Get Sprints For WorkSpace")]
	public async Task<Response<IEnumerable<SprintResponseDTO>>> GetWorkSpaceSprints([Service] ISprintService _sprintService, int workSpaceID) =>
		await _sprintService.GetWorkSpaceSprints(workSpaceID);

	[GraphQLDescription("Store opened sprint in cache")]
	public async Task<Response<IEnumerable<RecentSprintDTO>>> GetRecentOpenedSprints([Service] ISprintService _sprintService, string userId, string roomId)
		=> await _sprintService.GetRecentOpenedSprintsAsync(userId, roomId);

	[GraphQLDescription("Get Sprint by id")]
	public async Task<Response<Sprint>> GetSprint([Service] ISprintService _sprintService, string userId, string roomId, int sprintId)
		=> await _sprintService.GetSprint(userId,roomId,sprintId);
}

