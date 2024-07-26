using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Like
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public string? UserId { get; set; }

    public User? User { get; set; }

    [FirestoreProperty]
    public string? ProductId { get; set; }

    public Product? Product { get; set; }
}
