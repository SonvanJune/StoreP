using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class AddShippingMethodDto
{
    [Required]
    public required string Name { get; set;}

    [Required]
    public required string Location { get; set;}

    [Required]
    public required int Ensure { get; set;}

    [Required]
    public required int Price { get; set;}
}
