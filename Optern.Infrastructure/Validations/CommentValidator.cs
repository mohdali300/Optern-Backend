using FluentValidation;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
	public class CommentValidator:AbstractValidator<Comment>
	{
		public CommentValidator()
		{
			RuleFor(c => c.Content)
				.NotEmpty().WithMessage("Comment Cannot be empty!");

			RuleFor(c => c.Type)
				.NotEmpty().WithMessage("Type Cannot be empty!")
				.IsInEnum().WithMessage("Type must be valid content type!");
		}
	}
}
