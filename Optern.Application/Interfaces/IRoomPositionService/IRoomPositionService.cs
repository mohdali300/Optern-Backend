using Optern.Application.DTOs.Room;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IRoomTrackService
{
    public interface IRoomPositionService
    {
        public Task<Response<IEnumerable<CreateRoomDTO>>> GetPositionRooms(int positionId);

    }
}
