using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class ProductImage
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public required string Image { get; set; }

    [FirestoreProperty]
    public string? ProductId { get; set; }

    public Product? Product { get; set; }
}
