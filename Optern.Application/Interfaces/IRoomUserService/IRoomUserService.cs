using Optern.Application.DTOs.RoomUser;
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
        public Task<Response<RoomUserDTO>> DeleteCollaboratorAsync(string RoomId, string TargetUserId, string currentUserId);
        public Task<Response<RoomUserDTO>> ToggleLeadershipAsync(string roomId, string targetUserId, string currentUserId);

    }
}
