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
    public string? Name { get; set; }

    [FirestoreProperty]
    public string? Code{ get; set; }
}
