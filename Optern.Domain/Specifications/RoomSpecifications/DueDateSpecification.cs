namespace Optern.Domain.Specifications.RoomSpecifications
{
    public class DueDateSpecification : Specification<Domain.Entities.Task>
    {
        private readonly string? _dueDate;

        public DueDateSpecification(string? dueDate)
        {
            _dueDate = dueDate;
        }

        public override IQueryable<Entities.Task> Apply(IQueryable<Entities.Task> query)
        {
            if (string.IsNullOrEmpty(_dueDate)) return query;

            return query.Where(t => t.DueDate != null && t.DueDate.CompareTo(_dueDate) == 0);
        }
    }

}
