namespace DAL.Entities
{
    public class Post : Timestamp
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCommentable { get; set; } = true;

        public Guid AuthorId { get; set; }

        public virtual User Author { get; set; } = null!;
        public virtual ICollection<PostTag>? Tags { get; set; } = null!;
        public virtual ICollection<PostLike>? Likes { get; set; } = null!;
        public virtual ICollection<Comment>? Comments { get; set; } = null!;
        public virtual ICollection<PostAttach>? Attaches { get; set; } = null!;
    }
}