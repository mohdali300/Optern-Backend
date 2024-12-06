using FluentValidation;
using Microsoft.AspNetCore.Rewrite;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
	public class MessageValidator:AbstractValidator<Message>
	{
        public MessageValidator()
        {
            RuleFor(m => m.Content)
                .NotEmpty().WithMessage("Message Cannot be empty!");

            RuleFor(m => m.SentDate)
                .LessThanOrEqualTo(DateTime.Now);
		}

    }
}
