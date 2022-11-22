namespace DAL.Entities
{
    public class CommentLike : Like
    {
        public Guid CommentId { get; set; }

        public Comment Comment { get; set; } = null!;

        public CommentLike(Guid userId, Guid commentId)
        {
            UserId = userId;
            CommentId = commentId;
        }
    }
}
