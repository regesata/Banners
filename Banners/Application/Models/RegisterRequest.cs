using System.ComponentModel.DataAnnotations;

namespace Application.Models
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(30)]
        public string Email { get; set; }

        [Required]
        [MaxLength(64)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression("^(?i)(author|editor|moderator)$", ErrorMessage = "Invalid role. Only 'author', 'editor', or 'moderator' are acceptable.")]
        public string Role { get; set; }

    }
}
