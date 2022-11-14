using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Index(nameof(Email), IsUnique = true), Index(nameof(Nickname), IsUnique = true)]
    public class User : Timestamp
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; } = null!;
        public string? About { get; set; }

        public virtual Avatar? Avatar { get; set; }

        public virtual ICollection<Post>? Posts { get; set; } = null!;
        public virtual ICollection<Comment>? Comments { get; set; } = null!;
        public virtual ICollection<PostLike>? PostLikes { get; set; } = null!;
        public virtual ICollection<UserSession>? Sessions { get; set; } = null!;

        [InverseProperty("Following")]
        public virtual ICollection<Follower>? Followers { get; set; } = null!;

        [InverseProperty("Follewer")]
        public virtual ICollection<Follower>? Followings { get; set; } = null!;
    }
}
