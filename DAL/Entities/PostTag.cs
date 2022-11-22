namespace DAL.Entities
{
    public class PostTag : TagRelation
    {
        public Guid PostId { get; set; }

        public Post Post { get; set; } = null!;
    }
}
