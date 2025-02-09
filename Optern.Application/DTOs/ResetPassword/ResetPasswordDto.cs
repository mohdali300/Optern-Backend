

namespace Optern.Application.DTOs.ResetPassword
{
    public class ResetPasswordDto
    {
       

        [Required]
        public string Otp { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be 6 characters long.")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
