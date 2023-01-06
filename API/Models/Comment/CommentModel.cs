using API.Models.User;
using DAL.Entities;

namespace API.Models.Comment
{
    public class CommentModel : Timestamp
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }

        public UserModel Author { get; set; } = null!;
    }
}
