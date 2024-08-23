namespace StoreSp.Dtos.response;

public class ProductDto
{
    public required string Name { get; set; }
    public required string CreatedAt { get; set; }
    public UserDto? Author { get; set; }
    public List<CategoryDto>? Categories { get; set; }
    public required string Description { get; set; }
    public required string Code { get; set; }
    public required int Price { get; set; }
    public required int PriceSaleOff { get; set; }
    public required int SaleOff { get; set; }
    public required int QuantitySelled { get; set; }
    public int Likes { get; set; }
    public bool IsLiked { get; set; } = false;
    public List<ProductClassifyDto>? Classifies { get; set; }
    public List<string>? Images { get; set; }
}
