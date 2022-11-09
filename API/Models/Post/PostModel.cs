using API.Models.Attach;
using API.Models.User;
using DAL.Entities;

namespace API.Models.Post
{
    public class PostModel : Timestamp
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCommentable { get; set; } = true;

        public UserMiniModel Author { get; set; } = null!;
        public List<MetadataModel>? Files { get; set; }
    }
}
