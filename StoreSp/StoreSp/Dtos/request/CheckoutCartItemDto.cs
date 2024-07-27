using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CheckoutCartItemDto
{
    [Required]
    public required List<string> CartItemCode { get; set;}
}
