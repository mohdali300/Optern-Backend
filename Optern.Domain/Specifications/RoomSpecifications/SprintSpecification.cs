namespace Optern.Domain.Specifications.RoomSpecifications
{
    public class SprintSpecification : Specification<Domain.Entities.Task>
    {
        private readonly int? _sprintId;

        public SprintSpecification(int? sprintId)
        {
            _sprintId = sprintId;
        }
        public override IQueryable<Entities.Task> Apply(IQueryable<Entities.Task> query)
        {
            if (!_sprintId.HasValue) return query;
            return query.Where(t => t.SprintId == _sprintId.Value);
        }
    }
}
