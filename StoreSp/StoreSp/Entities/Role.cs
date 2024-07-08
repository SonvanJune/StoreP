using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Role
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    [FirestoreProperty]
    public required string Code{ get; set; }
}
