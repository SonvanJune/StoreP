using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateBillDto
{
    [Required] public required string Username { get; set; }
    [Required] public required string ShippingUnit { get; set; }
    [Required] public required string PaymentMethod { get; set; }
    [Required] public required int ShippingUnitPrice { get; set; }
}
