
	[ExtendObjectType("Query")]
	public class PositionQuery
	{
		[GraphQLDescription("Get All Positions")]
		public async Task<Response<List<PositionDTO>>> GetAllPositions([Service] IPositionService _positionService)
			=> await _positionService.GetAllAsync();
	}

