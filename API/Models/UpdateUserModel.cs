namespace API.Models;

public class UpdateUserModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public byte[]? Avatar { get; set; }
}