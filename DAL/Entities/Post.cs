namespace DAL.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCommentable { get; set; } = true;

        public User Author { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = null!;
    }
}