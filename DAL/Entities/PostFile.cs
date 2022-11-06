namespace DAL.Entities
{
    public class PostFile : Attach
    {
        public virtual Post Post { get; set; } = null!;
    }
}
