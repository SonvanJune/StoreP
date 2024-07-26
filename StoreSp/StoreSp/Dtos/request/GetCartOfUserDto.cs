using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class GetCartOfUserDto
{
    [Required]
    public required string Username { get; set;}
}
