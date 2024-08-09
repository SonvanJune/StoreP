using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Banner
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }
}
