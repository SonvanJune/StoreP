namespace StoreSp.Dtos.response;

public class CartItemDto
{
    public UserDto? Shop { get; set; }
    public ProductDto? Product { get; set; }
    public string? Code{ get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public int Total { get; set; }
    public string? CartItem_ProductClassifies{get; set; }
    public string? CartItem_ProductClassifyCodes{get; set; }
    public int Status { get; set; }
}
