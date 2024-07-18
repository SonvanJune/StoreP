using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class CartItem_ProductClassify
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public required string ProductClassify_Id { get; set; }

    [FirestoreProperty]
    public required string CartItem_Id { get; set; }

    public ProductClassify? ProductClassify { get; set; }
    public CartItem? CartItem { get; set; }
}
