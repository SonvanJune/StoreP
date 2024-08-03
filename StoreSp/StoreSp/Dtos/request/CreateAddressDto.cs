using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateAddressDto
{
    [Required]
    public required string Username { get; set; }
    
    public string? Description { get; set; }

    [Required]
    public required string Lat { get; set; }

    [Required]
    public required string Long { get; set; }

    [Required]
    public required string PhoneGet { get; set; }

    [Required]
    public required string NameGet { get; set; }

    [Required]
    public required string Status { get; set; }
}
