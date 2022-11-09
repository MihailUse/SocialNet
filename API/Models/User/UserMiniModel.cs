using API.Models.Attach;

namespace API.Models.User
{
    public class UserMiniModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = null!;
        public string? FullName { get; set; } = null!;

        public MetadataModel? Avatar { get; set; }
    }
}
