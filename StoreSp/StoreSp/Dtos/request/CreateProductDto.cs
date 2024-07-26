using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace StoreSp.Dtos.request;

public class CreateProductDto
{
    [Required]
    public required string Name { get; set; }

    public string Description { get; set; } = "";

    [Required]
    public required string ShippingUnit { get; set; }

    [Required]
    public required int Price { get; set; }

    [Required]
    public required int SaleOff { get; set; }

    [Required]
    public required string Auth { get; set; }

    [Required]
    public required string CategoryCode { get; set; }

    public required CreateProductClassifyDto[]? ClassiFies { get; set; } 
}
