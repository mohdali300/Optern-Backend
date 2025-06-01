

[ExtendObjectType("Mutation")]
    public class RoomMutation
    {

        [GraphQLDescription("Join To Room")]
        public async Task<Response<string>> JoinRoom([Service]IRoomService _roomService,JoinRoomDTO model)=>
            await _roomService.JoinToRoom(model);

        [GraphQLDescription("Create Room")]
        public async Task<Response<ResponseRoomDTO>> CreateRoom([Service] IRoomService _roomService, CreateRoomDTO model) =>
          await _roomService.CreateRoom(model);    
    
        [GraphQLDescription("Edit Room")]
        public async Task<Response<string>> EditRoom([Service] IRoomSettingService _roomSetting, string id, EditRoomDTO? model ) =>
          await _roomSetting.EditRoom(id,model!); 
    
        [GraphQLDescription("Edit Room Image")]
        public async Task<Response<string>> EditRoomImage([Service] IRoomSettingService _roomSetting, [ID]string id,[GraphQLType(typeof(UploadType))] IFile? CoverPicture) =>
          await _roomSetting.EditRoomImage(id,CoverPicture); 
    
        [GraphQLDescription("Delete Room")]
        public async Task<Response<bool>> DeleteRoom([Service] IRoomSettingService _roomSetting, string id) =>
          await _roomSetting.DeleteRoom(id);  
    
        [GraphQLDescription("Reset Room")]
        public async Task<Response<bool>> ResetRoom([Service] IRoomSettingService _roomSetting, string id) =>
          await _roomSetting.ResetRoom(id);

        [GraphQLDescription("Leave Room")]
        public async Task<Response<bool>> LeaveRoomAsync([Service] IRoomSettingService _roomSetting,string roomId, string userId)=>
          await _roomSetting.LeaveRoomAsync(roomId,userId);

       public async Task<Response<IEnumerable<ResponseRoomDTO>>> SearchForRoom([Service] IRoomService _roomService, string roomName)=>
        await _roomService.SearchForRoom(roomName);
      

}

