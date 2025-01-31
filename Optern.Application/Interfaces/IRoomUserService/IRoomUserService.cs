using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.RoomUset;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IRoomUserService
{
    public interface IRoomUserService
    {

        public Task<Response<List<RoomUserDTO>>> GetAllCollaboratorsAsync(string roomId, bool? isAdmin = null);
        public Task<Response<List<RoomUserDTO>>> GetPendingRequestsAsync(string roomId, string leaderId);
        public Task<Response<string>> RequestToRoom(JoinRoomDTO model);
        public Task<Response<RoomUserDTO>> DeleteCollaboratorAsync(string RoomId, string TargetUserId, string leaderId);
        public Task<Response<RoomUserDTO>> ToggleLeadershipAsync(string roomId, string targetUserId, string leaderId);
        public Task<Response<List<RoomUserDTO>>> AcceptRequestsAsync(string roomId, string leaderId, int? userRoomId = null, bool? approveAll = null);
        public Task<Response<string>> RejectRequestsAsync(string roomId, string leaderId, int? userRoomId = null, bool? rejectAll = null);
    }
}
