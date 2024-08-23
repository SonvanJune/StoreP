namespace StoreSp.Models;

public class Bill
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }

    public int? UserId { get; set; }

    public string? Code { get; set; }

    public required string PaymentMethod { get; set; }

    public int TotalProductPrice { get; set; }

    public int TotalPrice { get; set; }

    public int Quantity { get; set; }

    public required int Status { get; set; }

    public ICollection<Bill_Product>? Bill_Products { get; set; }

    public int? ShippingMethodId { get; set; }

    public ShippingMethod? ShippingMethod { get; set; }

    public int? AddressId { get; set; }

    public Address? Address { get; set; }
}
