namespace StoreSp.Dtos.response;

public class ProductDto
{
    public required string Name { get; set; }
    public required UserDto Author { get; set; }
    public required CategoryDto Category { get; set; }
    public string? Description { get; set; }
    public required int Price { get; set; }
    public required int PriceSaleOff { get; set; }
    public required int SaleOff { get; set; }
    public required string ShippingUnit { get; set; }
}
