using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Notification
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public string? UserId { get; set; }

    public User? User { get; set; }

    [FirestoreProperty]
    public int Status { get; set; }

    [FirestoreProperty]
    public int Type { get; set; }

    [FirestoreProperty]
    public string? Message { get; set; }
}
