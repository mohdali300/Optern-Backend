using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IRoomSkillService
{
    public interface IRoomSkillService
    {
        public  Task<Response<bool>> AddRoomSkills(string roomID, IEnumerable<int> data);
        public Task<Response<bool>> DeleteRoomSkills(string roomID, IEnumerable<int> skillIds);
    }
}
