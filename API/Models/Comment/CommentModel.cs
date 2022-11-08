using DAL.Entities;

namespace API.Models.Comment
{
    public class CommentModel : Timestamp
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;

        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }
    }
}
