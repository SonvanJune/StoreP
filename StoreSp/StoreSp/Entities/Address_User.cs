using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Address_User
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public string? UserId { get; set; }

    public User? User { get; set; }

    [FirestoreProperty]
    public string? AddressId { get; set; }

    public Address? Address { get; set; }
}
