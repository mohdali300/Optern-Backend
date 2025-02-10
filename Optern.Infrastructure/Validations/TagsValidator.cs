namespace Optern.Infrastructure.Validations
{
    public class TagsValidator : AbstractValidator<Tags>
    {
        public TagsValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(50)
                .WithMessage("Name cannot exceed 50 characters.");
        }
    }
}
