namespace Optern.Infrastructure.Validations
{
	public class ExperienceValidator:AbstractValidator<Experience>
	{
        public ExperienceValidator()
        {
            RuleFor(e => e.JobTitle)
                .NotEmpty().WithMessage("Job title Cannot be empty!")
                .MaximumLength(100).WithMessage("Job title cannot be more than 100 characters");

            RuleFor(e => e.Company)
                .NotEmpty().WithMessage("Comment Cannot be empty!")
                .MaximumLength(100).WithMessage("Job title cannot be more than 100 characters");

            RuleFor(e => e.StartDate)
                .NotEmpty().WithMessage("Start Date Cannot be empty!")
                .LessThanOrEqualTo(DateTime.Now);

            RuleFor(e => e.JobDescription)
                .MaximumLength(500).WithMessage("Job Description cannot be more than 500 characters");
		}
    }
}
