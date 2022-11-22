namespace API.Models.Tag
{
    public class SearchTagModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int FollowerCount { get; set; }
    }
}
