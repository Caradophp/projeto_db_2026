using projeto.Models.Enums;

namespace projeto.Models;

public class User
{
    
    public int Id { get; set; }
    public required string Name { get; set; } 

    public required string Email { get; set;}

    public required string Password { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserStatusEnum Status { get; set; }

}