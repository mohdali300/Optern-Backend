using FluentValidation;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
	public class PostValidator:AbstractValidator<Post>
	{
        public PostValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("Title Cannot be empty!")
                .MaximumLength(150).WithMessage("Title cannot be more than 150 characters");

            RuleFor(p => p.Content)
                .NotEmpty().WithMessage("Comment Cannot be empty!");

            RuleFor(p => p.CreatedDate).NotEmpty()
                .LessThanOrEqualTo(DateTime.Now);

		}
    }
}
