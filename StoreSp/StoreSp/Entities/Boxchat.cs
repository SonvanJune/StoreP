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
    public string? SenderId { get; set; }

    public User? Sender { get; set; }

    [FirestoreProperty]
    public string? ReceiverId { get; set; }

    public User? Receiver { get; set; }

    [FirestoreProperty]
    public string? Code { get; set; }

    [FirestoreProperty]
    public required int Status { get; set; }

    public List<Message>? Messages  { get; set; }
}
