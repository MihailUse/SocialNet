using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Attach
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; } // in bytes

        public Guid AuthorId { get; set; }

        public virtual User Author { get; set; } = null!;
    }
}
