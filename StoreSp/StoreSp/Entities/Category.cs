using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Category
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    public List<Product>? Products { get; set;}
}
