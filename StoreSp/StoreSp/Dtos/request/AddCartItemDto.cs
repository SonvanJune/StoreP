using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class AddCartItemDto
{
    [Required]
    public required string ProductCode { get; set;}

    public string? Status { get; set;}

    [Required]
    public required string Username { get; set;}

    [Required]
    public required int Quantity { get; set;}

    public List<string>? ProductClassifyCodes { get; set;}
}
