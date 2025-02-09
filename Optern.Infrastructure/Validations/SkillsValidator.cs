namespace Optern.Infrastructure.Validations
{
    public class SkillsValidator : AbstractValidator<Skills>
    {
        public SkillsValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name cannot exceed 100 characters.");

         
        }
    }
}
