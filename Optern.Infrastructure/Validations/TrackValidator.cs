namespace Optern.Infrastructure.Validations
{
    public class TrackValidator:AbstractValidator<Track>
    {
        public TrackValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty()
               .WithMessage("Name is required.")
               .MaximumLength(150)
               .WithMessage("Name cannot exceed 150 characters.");
        }
    }
}
