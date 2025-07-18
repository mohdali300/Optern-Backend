﻿namespace Optern.Infrastructure.Validations
{
    public class RoomValidator : AbstractValidator<Room>
    {
        public RoomValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Room name is required.")
                .MaximumLength(100).WithMessage("Room name must not exceed 100 characters.");

            RuleFor(r => r.Description)
                .NotEmpty().WithMessage("Room description is required.")
                .MaximumLength(800).WithMessage("Room description must not exceed 500 characters.");

            RuleFor(r => r.RoomType)
                .IsInEnum().WithMessage("Room type must be a valid enum value.");


            RuleFor(r => r.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt cannot be in the future.");

            RuleFor(r => r.CreatorId)
                .NotEmpty().WithMessage("Creator ID is required.");  
        }

        private bool IsValidImageFileName(string fileName)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            return allowedExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }

}
