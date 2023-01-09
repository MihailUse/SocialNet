using API.Models.Attach;
using API.Models.Comment;
using API.Models.User;
using DAL.Entities;

namespace API.Models.Post
{
    public class PostModel : Timestamp
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCommentable { get; set; } = true;
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLiked { get; set; }
        public CommentModel? PopularComment { get; set; }

        public UserModel Author { get; set; } = null!;
        public List<LinkMetadataModel>? Attaches { get; set; }
    }
}
