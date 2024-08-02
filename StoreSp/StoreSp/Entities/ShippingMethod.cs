using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class ShippingMethod
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    [FirestoreProperty]
    public string? Code { get; set; }

    [FirestoreProperty]
    public required string Location { get; set; }

    [FirestoreProperty]
    public required string Long { get; set; }

    [FirestoreProperty]
    public required string Lat { get; set; }

    [FirestoreProperty]
    public required int Ensure { get; set; }

    [FirestoreProperty]
    public required int Price { get; set; }

    [FirestoreProperty]
    public required int Status { get; set; }

    public List<Bill>? Bills { get; set; }
}
