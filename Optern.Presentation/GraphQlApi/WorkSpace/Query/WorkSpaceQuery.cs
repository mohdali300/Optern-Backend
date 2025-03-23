
using Optern.Domain.Entities;

[ExtendObjectType("Query")]

	public class WorkSpaceQuery
	{
		[GraphQLDescription("Get All WorkSpace")]
		public async Task<Response<IEnumerable<WorkSpace>>> GetRoomWorkSpaces([Service] IWorkSpaceService _workSpaceService,string roomId)
			=> await _workSpaceService.GetAllWorkSpace(roomId);
			
		[GraphQLDescription("Get WorkSpace By Id")]
		public async Task<Response<WorkSpace>> GetWorkSpace([Service] IWorkSpaceService _workSpaceService,int Id)
			=> await _workSpaceService.GetWorkSpace(Id);

	}
