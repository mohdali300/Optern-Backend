using FluentValidation;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
	public class NotificationValidator:AbstractValidator<Notifications>
	{
        public NotificationValidator()
        {
            RuleFor(n => n.Title)
                .MaximumLength(100).WithMessage("Title cannot be more than 100 characters");

            RuleFor(n => n.Message)
                .NotEmpty().WithMessage("Message content Cannot be empty!");

            RuleFor(n => n.CreatedTime).LessThanOrEqualTo(DateTime.Now);


		}
    }
}
