using System.ComponentModel.DataAnnotations;

namespace API.Models.Auth
{
    public class RequestTokenModel
    {
        [EmailAddress]
        public string Email { get; set; } = null!;

        [MinLength(4), MaxLength(64)]
        public string Password { get; set; } = null!;
    }
}