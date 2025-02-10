namespace Optern.Infrastructure.Validations
{
    public class VQuestionsValidator : AbstractValidator<VQuestions>
    {
        public VQuestionsValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Content is required.")
                .MaximumLength(500)
                .WithMessage("Content cannot exceed 500 characters.");

            RuleFor(x => x.Answer)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Answer must be non-negative.");
        }
    }
}
