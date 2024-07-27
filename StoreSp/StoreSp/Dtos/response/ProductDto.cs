namespace StoreSp.Dtos.response;

public class ProductDto
{
    public required string Name { get; set; }
    public UserDto? Author { get; set; }
    public List<CategoryDto>? Categories { get; set; }
    public required string Description { get; set; }
    public required string Code { get; set; }
    public required int Price { get; set; }
    public required int PriceSaleOff { get; set; }
    public required int SaleOff { get; set; }
    public required int QuantitySelled { get; set; }
    public required int Likes { get; set; }
    public required string ShippingUnit { get; set; }
    public List<ProductClassifyDto>? Classifies { get; set; }
}
