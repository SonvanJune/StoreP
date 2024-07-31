using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Address
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public string? Code { get; set; }

    [FirestoreProperty]
    public required string Description { get; set; }

    [FirestoreProperty]
    public required string PhoneGet { get; set; }

    [FirestoreProperty]
    public required string NameGet { get; set; }

    [FirestoreProperty]
    public required string Status { get; set; }

    public List<Bill>? Bills { get; set; }
}
