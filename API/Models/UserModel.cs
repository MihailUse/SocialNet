namespace API.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public byte[]? Avatar { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DetetedAt { get; set; }
}