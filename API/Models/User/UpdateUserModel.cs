using API.Models.Attach;
using System.ComponentModel.DataAnnotations;

namespace API.Models.User
{
    public class UpdateUserModel
    {
        [MinLength(2), MaxLength(64)]
        public string? Nickname { get; set; }
        public string? FullName { get; set; }
        public string? About { get; set; }
        public MetadataModel? Avatar { get; set; }
    }
}
