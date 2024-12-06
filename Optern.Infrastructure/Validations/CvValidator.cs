using FluentValidation;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
	public class CvValidator:AbstractValidator<CV>
	{
		public CvValidator()
		{
			RuleFor(c => c.Title)
				.NotEmpty().WithMessage("Title cannot be empty!")
				.MaximumLength(50).WithMessage("Title Cannot be more than 50 characters");
		}
	}
}
