namespace Optern.Infrastructure.Validations
{
    public class WorkSpaceValidator : AbstractValidator<WorkSpace>
    {
        public WorkSpaceValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MinimumLength(5)
                 .WithMessage("Min Length for Title 5 characters ")
                .MaximumLength(150)
                .WithMessage("Title cannot exceed 150 characters.");

            RuleFor(x => x.CreatedDate)
                .NotEmpty()
                .WithMessage("CreatedDate is required.");





        }
    }
}
