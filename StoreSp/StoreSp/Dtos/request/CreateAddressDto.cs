using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateAddressDto
{
    [Required]
    public required string Username { get; set;}
    [Required]
    public required string Description { get; set;}

    [Required]
    public required string PhoneGet { get; set;}

    [Required]
    public required string NameGet { get; set;}

    [Required]
    public required string Status { get; set; }
}
