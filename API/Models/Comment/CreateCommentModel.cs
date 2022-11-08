using System.ComponentModel.DataAnnotations;

namespace API.Models.Comment
{
    public class CreateCommentModel
    {
        [MinLength(2)]
        public string Text { get; set; } = null!;
        public Guid PostId { get; set; }
    }
}
