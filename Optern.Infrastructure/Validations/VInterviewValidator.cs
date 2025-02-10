namespace Optern.Infrastructure.Validations
{
    public class VInterviewValidator : AbstractValidator<VInterview>
    {
        public VInterviewValidator()
        {
            RuleFor(x => x.Category)
                .IsInEnum()
                .WithMessage("Category must be a valid enum value.");


            RuleFor(x => x.ScheduledTime)
                .NotEmpty()
                .WithMessage("ScheduledTime is required.")
                .GreaterThan(DateTime.Now)
                .WithMessage("ScheduledTime must be in the future.");


        }
    }
}
