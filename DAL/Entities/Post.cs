namespace DAL.Entities
{
    public class Post : Timestamp
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCommentable { get; set; } = true;

        public Guid AuthorId { get; set; }

        public User Author { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = null!;
    }
}