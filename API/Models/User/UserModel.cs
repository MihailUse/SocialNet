using API.Models.Attach;
using DAL.Entities;

namespace API.Models.User
{
    public class UserModel : Timestamp
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string? FullName { get; set; } = null!;
        public string? About { get; set; }

        public MetadataModel? Avatar { get; set; }
    }
}
