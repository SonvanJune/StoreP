using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateBillDto
{
    [Required] public required string Username { get; set; }
    [Required] public required string ShippingCode { get; set; }
    [Required] public required string AddressCode { get; set; }
    [Required] public required string PaymentMethod { get; set; }
    [Required] public required double Kilometers { get; set; }
}
