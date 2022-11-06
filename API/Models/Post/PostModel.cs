using API.Models.Attach;
using DAL.Entities;

namespace API.Models.Post
{
    public class PostModel : Timestamp
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCommentable { get; set; } = true;
        public Guid AuthorId { get; set; }

        public List<MetadataModel>? Files { get; set; }
    }
}
