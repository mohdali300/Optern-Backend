using FluentValidation;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Validations
{
	public class EducationValidator:AbstractValidator<Education>
	{
        public EducationValidator()
        {
            RuleFor(e => e.School)
                .NotEmpty().WithMessage("School cannot be empty!")
                .MaximumLength(150).WithMessage("School cannot be more than 150 character");

            RuleFor(e => e.University)
                .MaximumLength(150).WithMessage("University cannot be more than 150 character");

            RuleFor(e => e.Degree)
                .NotEmpty().WithMessage("Degree cannot be empty!")
                .IsInEnum().WithMessage("Degree must be valid degree type");

            RuleFor(e => e.StartYear)
                .NotEmpty().WithMessage("Start year cannot be empty!")
                .Must(BeValidStartYear).WithMessage($"Start Year must be valid (4 numbers, less or equal {DateTime.Now.Year})");

			RuleFor(e => e.EndYear)
				.NotEmpty().WithMessage("End year cannot be empty!")
				.Must(BeValidEndYear).WithMessage($"End Year must be valid (4 numbers)");
		}

        private bool BeValidStartYear(string year)
        {
            var isNumber= int.TryParse(year,out var number);
            if (!isNumber || year.Length != 4)
            {
                return false;
            }

            var currentYear=DateTime.Now.Year;
            return number <= currentYear ? true : false;
        }

		private bool BeValidEndYear(string year)
		{
			var isNumber = int.TryParse(year, out var number);
			if (!isNumber || year.Length != 4)
			{
				return false;
			}
            return true;
		}
	}
}
