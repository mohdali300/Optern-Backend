
    [ExtendObjectType("Mutation")]
    public class PositionMutation
    {
        [GraphQLDescription("Add Position For A Track")]
        public async Task<Response<PositionDTO>> AddPosition([Service] IPositionService _positionService,string name)
            => await _positionService.AddAsync(name);
    }
