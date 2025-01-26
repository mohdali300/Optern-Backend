using Microsoft.AspNetCore.Http;
using Optern.Application.DTOs.Room;
using Optern.Domain.Entities;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IRoomService
{
    public interface IRoomService
    {
        public Task<Response<IEnumerable<RoomDTO>>> GetAllAsync();
        public Task<Response<IEnumerable<RoomDTO>>> GetCreatedRooms(string id);
        public Task<Response<IEnumerable<RoomDTO>>> GetPopularRooms();
        public Task<Response<IEnumerable<RoomDTO>>> GetJoinedRooms(string id);
        public Task<Response<string>> JoinToRoom(JoinRoomDTO model);
        public Task<Response<RoomDTO>> CreateRoom(RoomDTO model , IFormFile CoverPicture);
    }
}
