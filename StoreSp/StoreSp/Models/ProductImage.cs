namespace StoreSp.Models;

public class ProductImage
{
    public int Id { get; set; }

    public required string Image { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }
}
