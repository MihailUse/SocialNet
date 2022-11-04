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

        public ICollection<Post> Posts { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = null!;

        [InverseProperty("Following")]
        public ICollection<Follower> Followers { get; set; } = null!;

        [InverseProperty("Follewer")]
        public ICollection<Follower> Followings { get; set; } = null!;
    }
}
