using Google.Cloud.Firestore;
using StoreSp.Entities;

namespace StoreSp;

[FirestoreData]
public class Product
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreateAt { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    [FirestoreProperty]
    public required string Description { get; set; }

    [FirestoreProperty]
    public required int Quantity { get; set; }

    [FirestoreProperty]
    public required int Active { get; set; }

    [FirestoreProperty]
    public required int Price { get; set; }

    [FirestoreProperty]
    public int PriceSaleOff { get; set; }

    [FirestoreProperty]
    public int SaleOff { get; set; }

    public User? Author { get; set; }

    [FirestoreProperty]
    public string? AuthorId { get; set; }

    public Category? Category { get; set; }

    [FirestoreProperty]
    public string? CategoryId { get; set; }
}
