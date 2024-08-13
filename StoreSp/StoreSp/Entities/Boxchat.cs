using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Boxchat
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public string? Code { get; set; }

    [FirestoreProperty]
    public required int Status { get; set; }
}
