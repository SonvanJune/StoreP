using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Token
{
    [FirestoreDocumentId]
    public required string Id { get; set; }

    [FirestoreProperty]
    public required string TokenValue { get; set; }

    [FirestoreProperty]
    public int Status{ get; set; }
}
