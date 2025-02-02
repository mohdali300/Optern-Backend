using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Specifications
{
    public abstract class Specification<T>
    {
        public abstract IQueryable<T> Apply(IQueryable<T> query);

        public Specification<T> And(Specification<T> other)
        {
            return new AndSpecification<T>(this, other);
        }
    }
}
