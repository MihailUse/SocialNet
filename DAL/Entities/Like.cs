namespace DAL.Entities
{
    public class Like
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
    }
}