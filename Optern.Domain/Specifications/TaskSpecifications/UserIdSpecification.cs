namespace Optern.Domain.Specifications.TaskSpecifications
{
	public class UserIdSpecification : Specification<Domain.Entities.Task>
	{
		private readonly string? _userId;

		public UserIdSpecification(string? userId)
		{
			_userId = userId;
		}

		public override IQueryable<Entities.Task> Apply(IQueryable<Entities.Task> query)
		{
			 if (string.IsNullOrEmpty(_userId)) return query;
			return query.Where(t => t.AssignedTasks.Any(u=>u.UserId == _userId));
		}
	}
}
