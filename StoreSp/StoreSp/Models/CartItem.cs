namespace StoreSp.Models;

public class CartItem
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public Product? Product { get; set; }

    public int ProductId { get; set; }

    public Cart? Cart { get; set; }

    public int CartId { get; set; }

    public int Price { get; set; }

    public required int Quantity { get; set; }

    public int Total { get; set; }

    public required int Status { get; set; }

    public ICollection<ProductClassify>? ProductClassifies { get; set;}
}
