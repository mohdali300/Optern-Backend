namespace Optern.Infrastructure.Validations
{
	public class CommentValidator:AbstractValidator<Comment>
	{
		public CommentValidator()
		{
			RuleFor(c => c.Content)
				.NotEmpty().WithMessage("Comment Cannot be empty!");

			RuleFor(c => c.Type)
				.NotEmpty().WithMessage("Type Cannot be empty!")
				.IsInEnum().WithMessage("Type must be valid content type!");
		}
	}
}
