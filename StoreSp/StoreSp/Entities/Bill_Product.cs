using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Bill_Product
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public required string ProductId { get; set; }

    [FirestoreProperty]
    public required string BillId { get; set; }

    [FirestoreProperty]
    public required int Quantity { get; set; }

    public Product? Product { get; set; }
    public Bill? Bill { get; set; }

    [FirestoreProperty]
    public string? ProductClassifies { get; set; }
}
