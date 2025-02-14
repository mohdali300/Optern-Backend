
namespace Optern.Domain.Specifications.TaskSpecifications
{
    public class AssigneeSpecification : Specification<Domain.Entities.Task>
    {
        private readonly string? _assigneeId;

        public AssigneeSpecification(string? assigneeId)
        {
            _assigneeId = assigneeId;
        }

        public override IQueryable<Entities.Task> Apply(IQueryable<Entities.Task> query)
        {
            if (string.IsNullOrEmpty(_assigneeId)) return query;

            return query.Where(t => t.AssignedTasks.Any(ut => ut.UserId == _assigneeId));
        }
    }


}
