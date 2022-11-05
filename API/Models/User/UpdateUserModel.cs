using System.ComponentModel.DataAnnotations;

namespace API.Models.User
{
    public class UpdateUserModel
    {
        [MinLength(2), MaxLength(64)]
        public string? Nickname { get; set; } = null!;

        public string? FullName { get; set; } = null!;
        public string? About { get; set; }
    }
}
