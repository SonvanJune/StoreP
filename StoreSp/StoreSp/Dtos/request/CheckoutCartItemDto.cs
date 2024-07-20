using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CheckoutCartItemDto
{
    [Required]
    public required string CartItemCode { get; set;}
}
