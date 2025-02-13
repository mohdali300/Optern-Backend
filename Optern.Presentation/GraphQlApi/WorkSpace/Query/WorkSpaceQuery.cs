
using Optern.Domain.Entities;

[ExtendObjectType("Query")]

	public class WorkSpaceQuery
	{
		[GraphQLDescription("Get All WorkSpace")]
		public async Task<Response<List<WorkSpace>>> GetRoomWorkSpaces([Service] IWorkSpaceService _workSpaceService,string roomId)
			=> await _workSpaceService.GetAllWorkSpace(roomId);

    }
