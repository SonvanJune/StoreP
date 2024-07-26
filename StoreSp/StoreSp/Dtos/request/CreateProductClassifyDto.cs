using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateProductClassifyDto
{
    [Required]
    public required string GroupName { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Image { get; set; }
    [Required]
    public required int Quantity { get; set; }
    [Required]
    public required int IncreasePercent { get; set; }
}
