
namespace Optern.Application.Interfaces.IWorkSpaceService
{
	public interface IWorkSpaceService
	{
		public Task<Response<WorkSpaceDTO>> CreateWorkSpace(WorkSpaceDTO model);
		public Task<Response<WorkSpaceDTO>> UpdateWorkSpace(int id, string title);
		public Task<Response<bool>> DeleteWorkSpace(int id);
		public Task<Response<IEnumerable<WorkSpace>>> GetAllWorkSpace(string roomId);
		public Task<Response<WorkSpace>> GetWorkSpace(int id);
	}
}
