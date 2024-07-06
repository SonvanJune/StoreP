using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateUserDto
{
    [Required]
    public required string Name { get; set;}
}
