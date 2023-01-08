using API.Models.Attach;
using DAL.Entities;

namespace API.Models.User
{
    public class UserProfileModel : Timestamp
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = null!;
        public string? FullName { get; set; } = null!;
        public string? About { get; set; }
        public bool IsFollowing { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public int PostCount { get; set; }

        public LinkMetadataModel? Avatar { get; set; }
    }
}
