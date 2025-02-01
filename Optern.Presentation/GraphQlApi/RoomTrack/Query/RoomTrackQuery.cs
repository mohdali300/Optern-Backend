
    [ExtendObjectType("Query")]
    public class RoomTrackQuery
    {
        [GraphQLDescription("Get all room for specific subtrack")]
        public async Task<Response<IEnumerable<CreateRoomDTO>>> GetPositionRooms([Service] IRoomPositionService _roomTrackService, int positionId)
            =>await _roomTrackService.GetPositionRooms(positionId);

    }

