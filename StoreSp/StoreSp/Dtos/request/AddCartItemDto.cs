using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class AddCartItemDto
{
    [Required]
    public required string ProductCode { get; set;}

    [Required]
    public required string UserId { get; set;}

    [Required]
    public required int Quantity { get; set;}

    public List<string>? ProductClassifyCodes { get; set;}
}
