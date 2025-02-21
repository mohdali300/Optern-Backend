

namespace Optern.Application.Interfaces.ISprintService
{
	public interface ISprintService
	{
		public  Task<Response<IEnumerable<SprintResponseDTO>>> GetWorkSpaceSprints(int workSpaceId);
		public Task<Response<SprintResponseDTO>> AddSprint(AddSprintDTO model);
		public Task<Response<SprintResponseDTO>> EditSprint(int id, EditSprintDTO model);
		public Task<Response<bool>> DeleteSprint(int id);
		public Task<Response<IEnumerable<RecentSprintDTO>>> GetRecentOpenedSprintsAsync(string userId, string roomId);
		public Task<Response<Sprint>> GetSprint(string userId, string roomId, int id);
	}
}
