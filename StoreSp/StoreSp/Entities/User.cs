namespace StoreSp.Entities;
using Google.Cloud.Firestore;

[FirestoreData]
public class User
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreateAt { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    [FirestoreProperty]
    public required string Email { get; set; }

    [FirestoreProperty]
    public required string Phone { get; set; }

    [FirestoreProperty]
    public required string Password { get; set; }

    [FirestoreProperty]
    public int Status { get; set; }

    [FirestoreProperty]
    public int Active { get; set; }

    [FirestoreProperty]
    public required string Avatar { get; set; }

    public Role? role { get; set; }

    [FirestoreProperty]
    public string? roleId { get; set; }
}
