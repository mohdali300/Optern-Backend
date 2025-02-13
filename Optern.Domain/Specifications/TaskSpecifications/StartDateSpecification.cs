namespace Optern.Domain.Specifications.TaskSpecifications
{
public class StartDateSpecification : Specification<Domain.Entities.Task>
    {
        private readonly string? _startDate;

        public StartDateSpecification(string? startDate)
        {
            _startDate = startDate;
        }

        public override IQueryable<Entities.Task> Apply(IQueryable<Entities.Task> query)
        {
            if (string.IsNullOrEmpty(_startDate)) return query;
            return query.Where(t => t.StartDate != null && t.StartDate.CompareTo(_startDate) >= 0);
        }
    }



}
