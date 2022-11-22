namespace API.Models.Tag
{
    public class TagInfoModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public int PostCount { get; set; }
        public int FollowerCount { get; set; }
    }
}
