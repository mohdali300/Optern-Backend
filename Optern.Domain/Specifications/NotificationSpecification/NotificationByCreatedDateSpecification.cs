
using Optern.Domain.Entities;

namespace Optern.Domain.Specifications.NotificationSpecification
{
    public class NotificationByCreatedDateSpecification : Specification<UserNotification>
    {
        private readonly DateTime? _createdDate;
        private readonly bool _isDescending; 

        public NotificationByCreatedDateSpecification(DateTime? createdDate, bool isDescending = true)
        {
            _createdDate = createdDate?.Date;
            _isDescending = isDescending;
        }

        public override IQueryable<UserNotification> Apply(IQueryable<UserNotification> query)
        {
            if (_createdDate.HasValue)
            {
                query = query.Where(n => n.Notifications.CreatedTime.Date == _createdDate.Value);
            }

            query = _isDescending
                ? query.OrderByDescending(n => n.Notifications.CreatedTime)
                : query.OrderBy(n => n.Notifications.CreatedTime);

            return query;
        }
    }


}
