using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Specifications
{
    public class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _first;
        private readonly Specification<T> _second;

        public AndSpecification(Specification<T> first, Specification<T> second)
        {
            _first = first;
            _second = second;
        }

        public override IQueryable<T> Apply(IQueryable<T> query)
        {
            query = _first.Apply(query);
            return _second.Apply(query);
        }
    }
}
