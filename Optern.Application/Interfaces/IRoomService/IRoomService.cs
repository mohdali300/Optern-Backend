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
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetAllAsync();
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetCreatedRooms(string id);
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetPopularRooms();
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetJoinedRooms(string id);
        public Task<Response<string>> JoinToRoom(JoinRoomDTO model);

        public Task<Response<ResponseRoomDTO>> CreateRoom(CreateRoomDTO model , IFile CoverPicture);
        public Task<Response<ResponseRoomDTO>> GetRoomById(string id);
    }
}
