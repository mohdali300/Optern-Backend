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


		}
    }
}
