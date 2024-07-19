namespace StoreSp.Dtos.response;

public class CartDto
{
    public UserDto? User { get; set; }
    public int TotalPrice { get; set; }
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>(); 
}
