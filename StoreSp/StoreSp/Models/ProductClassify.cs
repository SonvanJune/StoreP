namespace StoreSp.Models;

public class ProductClassify
{
    public int Id { get; set; }

    public required string GroupName { get; set; }
    
    public string? Code { get; set; }

    public required string Name { get; set; }

    public required string Image { get; set; }

    public required int Quantity { get; set; }

    public required int IncreasePercent { get; set; }

    public required int Status { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public ICollection<CartItem>? Items { get; set;}
}
