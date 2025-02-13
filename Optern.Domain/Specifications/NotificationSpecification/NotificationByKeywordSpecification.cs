
using Microsoft.EntityFrameworkCore;
using Optern.Domain.Entities;

namespace Optern.Domain.Specifications.NotificationSpecification
{

    public class NotificationByKeywordSpecification : Specification<UserNotification>
    {
        private readonly string _keyword;

        public NotificationByKeywordSpecification(string keyword)
        {
            _keyword = keyword?.Trim() ?? string.Empty; 
        }

        public override IQueryable<UserNotification> Apply(IQueryable<UserNotification> query)
        {
            return string.IsNullOrWhiteSpace(_keyword) ? query :
             query.Where(n =>
                 EF.Functions.Like(n.Notifications.Title, $"%{_keyword}%") ||
                 EF.Functions.Like(n.Notifications .Message, $"%{_keyword}%"));
        
        }
    }


}
