namespace API.Models.Post
{
    public class CreatePostModel
    {
        public string Text { get; set; } = null!;
        public bool IsCommentable { get; set; } = true;
    }
}
