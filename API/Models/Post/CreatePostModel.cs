using API.Models.Attach;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Post
{
    public class CreatePostModel
    {
        public string Text { get; set; } = null!;
        public bool IsCommentable { get; set; } = true;

        [MaxLength(15)]
        public List<MetadataModel>? Attaches { get; set; }
    }
}
