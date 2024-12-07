using FluentValidation;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
    public class VFeedBackValidator : AbstractValidator<VFeedBack>
    {
        public VFeedBackValidator()
        {
            RuleFor(x => x.PerformanceScore)
                .InclusiveBetween(0, 100)
                .WithMessage("PerformanceScore must be between 0 and 100.");

          
        }
    }
}
