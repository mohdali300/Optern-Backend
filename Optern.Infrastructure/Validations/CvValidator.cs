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
