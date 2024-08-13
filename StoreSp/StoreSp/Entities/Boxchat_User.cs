using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Boxchat_User
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public string? UserId { get; set; }

    public User? User { get; set; }

    [FirestoreProperty]
    public string? BoxchatId { get; set; }

    public Boxchat? Boxchat { get; set; }
}
