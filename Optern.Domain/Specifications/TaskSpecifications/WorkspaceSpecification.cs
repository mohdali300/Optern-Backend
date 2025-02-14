namespace Optern.Domain.Specifications.TaskSpecifications
{
    public class WorkspaceSpecification : Specification<Domain.Entities.Task>
    {
        private readonly int? _workspaceId;

        public WorkspaceSpecification(int? workspaceId)
        {
            _workspaceId = workspaceId;
        }

        public override IQueryable<Entities.Task> Apply(IQueryable<Entities.Task> query)
        {
            if (!_workspaceId.HasValue) return query;
            return query.Where(t => t.Sprint.WorkSpaceId == _workspaceId.Value);
        }
    }
}
