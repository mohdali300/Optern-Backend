using FluentValidation;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
    public class TrackValidator:AbstractValidator<Track>
    {
        public TrackValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty()
               .WithMessage("Name is required.")
               .MaximumLength(150)
               .WithMessage("Name cannot exceed 150 characters.");
        }
    }
}
