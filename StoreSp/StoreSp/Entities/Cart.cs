using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Cart
{
    [FirestoreDocumentId]
    public string? Id { get; set; }
    
    [FirestoreProperty]
    public string? UserId { get; set; }

    public User? User { get; set; }

    public List<CartItem>? Items { get; set; }

    [FirestoreProperty]
    public int TotalPrice { get; set; }
}
