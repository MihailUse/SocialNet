namespace DAL.Entities
{
    public class PostLike : Like
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;

        public PostLike(Guid userId, Guid postId)
        {
            UserId = userId;
            PostId = postId;
        }
    }
}
