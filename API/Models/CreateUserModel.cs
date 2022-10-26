using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class CreateUserModel
{
    public string Name { get; set; }
    public string Password { get; set; }
    [Compare(nameof(Password))] 
    public string RetryPassword { get; set; }
    public byte[]? Avatar { get; set; }
}