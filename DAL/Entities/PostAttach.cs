namespace DAL.Entities
{
    public class PostAttach : Attach
    {
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
    }
}
