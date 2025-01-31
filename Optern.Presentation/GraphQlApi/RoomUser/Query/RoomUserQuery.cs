
    [ExtendObjectType("Query")]
    public class RoomUserQuery
    {
        [GraphQLDescription("All Room's Collaborators")]
        public async Task<Response<List<RoomUserDTO>>> GetAllCollaboratorsAsync([Service] IRoomUserService _roomUserService, string roomId, bool? isAdmin = null)
            => await _roomUserService.GetAllCollaboratorsAsync(roomId, isAdmin);

        [GraphQLDescription("All Pending Requests")]
        public async Task<Response<List<RoomUserDTO>>> GetPendingRequestsAsync([Service] IRoomUserService _roomUserService, string roomId, string leaderId)
           => await _roomUserService.GetPendingRequestsAsync(roomId, leaderId);
    }

