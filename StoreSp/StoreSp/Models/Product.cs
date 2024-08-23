namespace StoreSp.Models;

public class Product
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public required string Name { get; set; }

    public string? Code { get; set; }

    public required string Description { get; set; }

    public required int Price { get; set; }

    public required int QuantitySelled { get; set; }

    public int PriceSaleOff { get; set; }

    public required int SaleOff { get; set; }

    public required int Active { get; set; }

    public User? Author { get; set; }

    public int AuthorId { get; set; }

    public ICollection<Category>? Categories { get; set; }

    public ICollection<ProductClassify>? ProductClassifies { get; set; }

    public ICollection<ProductImage>? ProductImages { get; set; }

    public ICollection<Bill_Product>? Bill_Products { get; set; }
    public ICollection<Like>? Likes { get; set; }
}
