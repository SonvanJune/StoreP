using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Bill
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }

    public User? User { get; set; }

    [FirestoreProperty]
    public string? UserId { get; set; }

    [FirestoreProperty]
    public string? Code { get; set; }

    [FirestoreProperty]
    public required string ShippingUnit { get; set; }

    [FirestoreProperty]
    public required string PaymentMethod { get; set; }

    [FirestoreProperty]
    public required int ShippingUnitPrice { get; set; }

    [FirestoreProperty]
    public int TotalProductPrice { get; set; }

    [FirestoreProperty]
    public int TotalPrice { get; set; }

    [FirestoreProperty]
    public int Quantity { get; set; }

    [FirestoreProperty]
    public required int Status { get; set; }

    public List<Product>? Products { get; set; }
}
