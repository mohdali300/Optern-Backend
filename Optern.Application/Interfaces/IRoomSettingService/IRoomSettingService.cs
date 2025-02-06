using Optern.Application.DTOs.Room;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IRoomSettingService
{
    public interface IRoomSettingService
    {
        public Task<Response<string>> EditRoom(string id, EditRoomDTO model);
        public Task<Response<string>> EditRoomImage(string roomId, [GraphQLType(typeof(UploadType))] IFile? CoverPicture);
        public Task<Response<bool>> DeleteRoom(string roomId);
        public Task<Response<bool>> ResetRoom(string roomId);
    }
}
