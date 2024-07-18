using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class ProductClassify
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public required string GroupName { get; set; }
    
    [FirestoreProperty]
    public string? Code { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    [FirestoreProperty]
    public required string Image { get; set; }

    [FirestoreProperty]
    public required int Quantity { get; set; }

    [FirestoreProperty]
    public required int IncreasePercent { get; set; }

    [FirestoreProperty]
    public required int Status { get; set; }

    [FirestoreProperty]
    public string? ProductId { get; set; }

    public Product? Product { get; set; }

    [FirestoreProperty]
    public required bool IsChoose { get; set; } = false;
}
