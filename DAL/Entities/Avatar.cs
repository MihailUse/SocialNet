namespace DAL.Entities
{
    public class Avatar : Attach
    {
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
