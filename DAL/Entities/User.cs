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
        public string? FullName { get; set; }
        public string? About { get; set; }
        public string? NotificationToken { get; set; }

        public virtual Avatar? Avatar { get; set; }

        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<PostLike>? PostLikes { get; set; }
        public virtual ICollection<CommentLike>? CommentLikes { get; set; }
        public virtual ICollection<UserSession>? Sessions { get; set; }
        public virtual ICollection<UserTag>? FollowTags { get; set; }

        [InverseProperty(nameof(Follower.Following))]
        public virtual ICollection<Follower>? Followers { get; set; }

        [InverseProperty(nameof(Follower.Follewer))]
        public virtual ICollection<Follower>? Followings { get; set; }
    }
}
