using FluentValidation;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
    public class RoomValidator : AbstractValidator<Room>
    {
        public RoomValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Room name is required")
                .MaximumLength(100).WithMessage("Room name must not exceed 100 characters");

            RuleFor(r => r.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0");

            RuleFor(r => r.RoomType)
            .NotEmpty().WithMessage("Room Type Cannot be empty!")
            .IsInEnum().WithMessage("Room Type must be valid content type!");
        }
    }
}
