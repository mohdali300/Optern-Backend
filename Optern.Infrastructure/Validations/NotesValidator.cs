using FluentValidation;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
	public class NotesValidator:AbstractValidator<Notes>
	{
        public NotesValidator()
        {
            RuleFor(n => n.Content)
                .NotEmpty().WithMessage("COntent Cannot be empty!");

            RuleFor(n => n.UpdatedAt).GreaterThanOrEqualTo(n => n.CreatedAt)
                .WithMessage("Updated time cannot be earlier than Created time.");

		}
    }
}
