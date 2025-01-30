
    [ExtendObjectType("Query")]
    public class RoomTrackQuery
    {
        [GraphQLDescription("Get all room for specific subtrack")]
        public async Task<Response<IEnumerable<CreateRoomDTO>>> GetSubTrackRooms([Service] IRoomTrackService _roomTrackService, int subTrackId)
            =>await _roomTrackService.GetSubTrackRooms(subTrackId);

    }

