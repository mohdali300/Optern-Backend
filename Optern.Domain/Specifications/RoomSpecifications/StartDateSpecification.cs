using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Optern.Domain.Specifications.RoomSpecifications
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
