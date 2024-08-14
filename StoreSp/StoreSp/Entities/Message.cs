using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Message
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public string? SenderId { get; set; }

    public User? Sender { get; set; }

    [FirestoreProperty]
    public string? ReceiverId { get; set; }

    public User? Receiver { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }
    
    [FirestoreProperty]
    public required string Text { get; set; }

    [FirestoreProperty]
    public required int Status { get; set; }

    [FirestoreProperty]
    public string? BoxchatId { get; set; }

    public Boxchat? Boxchat { get; set; }
}
