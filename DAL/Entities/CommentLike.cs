namespace DAL.Entities
{
    public class CommentLike
    {
        public Guid UserId { get; set; }
        public Guid CommentId { get; set; }

        public User User { get; set; } = null!;
        public Comment Comment { get; set; } = null!;

        public CommentLike(Guid userId, Guid commentId)
        {
            UserId = userId;
            CommentId = commentId;
        }
    }
}
