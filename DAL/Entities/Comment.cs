namespace DAL.Entities
{
    public class Comment : Timestamp
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;

        public Guid AuthorId { get; set; }
        public Guid PostId { get; set; }

        public Post Post { get; set; } = null!;
        public User Author { get; set; } = null!;
    }
}