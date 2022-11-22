using Microsoft.EntityFrameworkCore;

namespace DAL.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PostTag>? PostTags { get; set; }
        public virtual ICollection<UserTag>? UserTags { get; set; }

        public Tag(string name)
        {
            Name = name;
        }
    }
}
