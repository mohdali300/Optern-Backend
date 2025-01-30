using FluentValidation;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
    public class SprintValidator : AbstractValidator<Sprint>
    {
        public SprintValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(150)
                .WithMessage("Title cannot exceed 150 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("StartDate is required.")
                .LessThan(x => x.EndDate)
                .WithMessage("StartDate must be earlier than EndDate.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("EndDate is required.")
                .GreaterThan(x => x.StartDate)
                .WithMessage("EndDate must be Greater than StartDate");

            RuleFor(x => x.WorkSpaceId)
                .NotNull()
                .NotEmpty()
                 .WithMessage("WorkSpace Filed is required.")
                .NotEqual(0)
                .WithMessage("Invalid WorkSpace");



        }
    }
}
