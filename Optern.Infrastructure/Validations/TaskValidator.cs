﻿using Task = Optern.Domain.Entities.Task;

namespace Optern.Infrastructure.Validations
{
    public class TaskValidator : AbstractValidator<Task>
    {
        public TaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(150)
                .WithMessage("Title cannot exceed 150 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("StartDate is required.")
                .LessThan(x => x.DueDate)
                .WithMessage("StartDate must be earlier than DueDate.");

            RuleFor(x => x.DueDate)
                .NotEmpty()
                .WithMessage("DueDate is required.")
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("DueDate must be earlier than or equal to EndDate.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("EndDate is required if the task is completed.");


        }
    }
}
