namespace Optern.Infrastructure.Validations
{
	public class PTPFeedBackValidator:AbstractValidator<PTPFeedBack>
	{
        public PTPFeedBackValidator()
        {
            RuleFor(f => f.InterviewerPerformance)
                .NotEmpty().WithMessage("Interviewer Performance feedback Cannot be empty!");

			RuleFor(f => f.IntervieweePerformance)
				.NotEmpty().WithMessage("Interviewee Performance feedback Cannot be empty!");

		}
    }
}
