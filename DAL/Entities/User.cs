using Microsoft.EntityFrameworkCore;

namespace DAL.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : Timestamp
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; } = null!;
        public string? About { get; set; }

        public ICollection<Post> Posts { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = null!;
        public ICollection<Follower> Followers { get; set; } = null!;
        public ICollection<Follower> Followings { get; set; } = null!;
    }
}
