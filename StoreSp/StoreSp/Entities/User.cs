namespace StoreSp.Entities;
using Google.Cloud.Firestore;


[FirestoreData]
public class User
{
    [FirestoreDocumentId]
    public string? Id { get; set; }
    [FirestoreProperty]
    public required string Name { get; set; }
}
