using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class CartItem
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty]
    public string? Code { get; set; }

    public Product? Product { get; set; }

    [FirestoreProperty]
    public string? ProductId { get; set; }

    public Cart? Cart { get; set; }

    [FirestoreProperty]
    public string? CartId { get; set; }

    [FirestoreProperty]
    public int Price { get; set; }

    [FirestoreProperty]
    public required int Quantity { get; set; }

    [FirestoreProperty]
    public int Total { get; set; }

    [FirestoreProperty]
    public required int Status { get; set; }

    public List<CartItem_ProductClassify>? CartItem_ProductClassifies { get; set;}
}
