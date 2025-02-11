namespace Optern.Infrastructure.Validations
{
	public class MessageValidator:AbstractValidator<Message>
	{
        public MessageValidator()
        {
            RuleFor(m => m.Content)
                .NotEmpty().WithMessage("Message Cannot be empty!");

            RuleFor(m => m.SentAt)
                .LessThanOrEqualTo(DateTime.Now);
		}

    }
}
