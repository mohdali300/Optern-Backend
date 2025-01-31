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
        public Task<Response<IEnumerable<CreateRoomDTO>>> GetAllAsync();
        public Task<Response<IEnumerable<CreateRoomDTO>>> GetCreatedRooms(string id);
        public Task<Response<IEnumerable<CreateRoomDTO>>> GetPopularRooms();
        public Task<Response<IEnumerable<CreateRoomDTO>>> GetJoinedRooms(string id);
        public Task<Response<string>> JoinToRoom(JoinRoomDTO model);
        

        public Task<Response<CreateRoomDTO>> CreateRoom(CreateRoomDTO model , IFile CoverPicture);
        public Task<Response<RoomDetailsDTO>> GetRoomById(string id);
    }
}
