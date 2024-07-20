using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Product
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    [FirestoreProperty]
    public string? Code { get; set; }

    [FirestoreProperty]
    public required string Description { get; set; }

    [FirestoreProperty]
    public required string ShippingUnit { get; set; }

    [FirestoreProperty]
    public required int Price { get; set; }

    [FirestoreProperty]
    public int PriceSaleOff { get; set; }

    [FirestoreProperty]
    public required int SaleOff { get; set; }

    [FirestoreProperty]
    public required int Active { get; set; }

    public User? Author { get; set; }

    [FirestoreProperty]
    public string? AuthorId { get; set; }

    public List<Category>? Categories { get; set; }

    public List<ProductClassify>? ProductClassifies { get; set; }

    public List<Bill>? Checkouts { get; set; }
}
