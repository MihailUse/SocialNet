namespace DAL.Entities
{
    public class TagRelation
    {
        public Guid TagId { get; set; }

        public Tag Tag { get; set; } = null!;
    }
}
