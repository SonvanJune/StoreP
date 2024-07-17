using Google.Cloud.Firestore;

namespace StoreSp.Entities;

[FirestoreData]
public class Category
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    [FirestoreProperty]
    public required string Code { get; set; }

    [FirestoreProperty]
    public int Level { get; set; }
    
    [FirestoreProperty]
    public string Avatar { get; set; } = null!;

    public List<Product>? Products { get; set; }

    [FirestoreProperty]
    public string ParentCategoryId { get; set; } = null!;

    public Category? ParentCategory { get; set; }
}
