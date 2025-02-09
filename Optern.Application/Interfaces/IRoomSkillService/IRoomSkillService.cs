

namespace Optern.Application.Interfaces.IRoomSkillService
{
    public interface IRoomSkillService
    {
        public  Task<Response<bool>> AddRoomSkills(string roomID, IEnumerable<int> data);
        public Task<Response<bool>> DeleteRoomSkills(string roomID, IEnumerable<int> skillIds);
    }
}
