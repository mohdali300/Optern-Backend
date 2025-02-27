namespace Optern.Infrastructure.Validations
{
	public class PTPInterviewValidator:AbstractValidator<PTPInterview>
	{
        public PTPInterviewValidator()
        {
            RuleFor(i => i.Category)
                .NotEmpty().WithMessage("Category Cannot be empty!")
                .IsInEnum().WithMessage("Category must be valid interview category");

            
            RuleFor(i => i.Status)
                .NotEmpty().WithMessage("Interview status Cannot be empty!")
                .IsInEnum().WithMessage("Status must be valid interview status");

		}
    }
}
