namespace Optern.Infrastructure.Validations
{
    public class VInterviewValidator : AbstractValidator<VInterview>
    {
        public VInterviewValidator()
        {
            RuleFor(x => x.Category)
                .IsInEnum()
                .WithMessage("Category must be a valid enum value.");

            RuleFor(x => x.QusestionType)
                .IsInEnum()
                .WithMessage("QusestionType must be a valid enum value.");





        }
    }
}
