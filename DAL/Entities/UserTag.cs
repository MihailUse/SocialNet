namespace DAL.Entities
{
    public class UserTag : TagRelation
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public UserTag(Guid tagId, Guid userId)
        {
            TagId = tagId;
            UserId = userId;
        }
    }
}
