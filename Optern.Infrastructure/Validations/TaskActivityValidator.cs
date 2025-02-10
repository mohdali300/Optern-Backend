namespace Optern.Infrastructure.Validations
{
    public class TaskActivityValidator : AbstractValidator<TaskActivity>
    {
        public TaskActivityValidator()
        {
            RuleFor(activity => activity.Content)
           .NotEmpty().WithMessage("Content is required.")
           .MaximumLength(500).WithMessage("Content must be less than 500 characters.");
        }
    }
}
