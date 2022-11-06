namespace DAL.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid RefreshTokenId { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;

        public UserSession(Guid userId)
        {
            UserId = userId;
        }
    }
}
