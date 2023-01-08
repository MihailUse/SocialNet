using API.Models.Attach;
using DAL.Entities;

namespace API.Models.User
{
    public class UserModel : Timestamp
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = null!;
        public string? FullName { get; set; }
        public string? AvatarLink { get; set; }

        public LinkMetadataModel? Avatar { get; set; }
    }
}
