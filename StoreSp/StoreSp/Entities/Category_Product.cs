using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Category_Product
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public required string ProductId { get; set; }

    [FirestoreProperty]
    public required string CategoryId { get; set; }

    public Product? Product { get; set; }
    public Category? Category { get; set; }
}
