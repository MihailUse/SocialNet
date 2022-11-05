using System.ComponentModel.DataAnnotations;

namespace API.Models.User
{
    public class CreateUserModel
    {
        [EmailAddress]
        public string Email { get; set; } = null!;

        [MinLength(2), MaxLength(64)]
        public string Nickname { get; set; } = null!;

        [MinLength(4), MaxLength(64)]
        public string Password { get; set; } = null!;

        [Compare(nameof(Password))]
        public string PasswordRetry { get; set; } = null!;
    }
}
