using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateUserDto
{
    [Required]
    public required string Name { get; set;}

    [Required]
    public required string Email { get; set;}

    [Required]
    public required string Phone { get; set;}

    [Required]
    public required string Password { get; set;}

    [Required]
    public required string Avatar { get; set;}

    [Required]
    public required string RoleId { get; set;}
}
