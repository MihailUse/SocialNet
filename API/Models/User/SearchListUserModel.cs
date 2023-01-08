using API.Models.Attach;

namespace API.Models.User
{
    public class SearchListUserModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = null!;
        public string? FullName { get; set; } = null!;
        public int FollowerCount { get; set; }
        public string? AvatarLink { get; set; }

        public LinkMetadataModel? Avatar { get; set; }
    }
}
