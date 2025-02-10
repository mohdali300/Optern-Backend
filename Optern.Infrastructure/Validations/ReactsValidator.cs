namespace Optern.Infrastructure.Validations
{
    public class ReactsValidator : AbstractValidator<Reacts>
    {
        public ReactsValidator()
        {
            RuleFor(x => x.ReactDate)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("React date cannot be in the future.");


            RuleFor(x => x.ReactType)
                .IsInEnum()
                .WithMessage("ReactType must be a valid enum value.");

            
        }
    }
}
