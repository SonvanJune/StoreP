namespace StoreSp.Entities;
using Google.Cloud.Firestore;

[FirestoreData]
public class User
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreDocumentCreateTimestamp]
    public Timestamp CreateAt { get; set; }

    [FirestoreDocumentUpdateTimestamp]
    public Timestamp UpdateAt { get; set; }

    [FirestoreProperty]
    public required string Name { get; set; }

    [FirestoreProperty]
    public string Email { get; set; } = null!;

    [FirestoreProperty]
    public string Phone { get; set; } = null!;

    [FirestoreProperty]
    public string PasswordHash { get; set; } = null!;

    [FirestoreProperty]
    public int Status { get; set; }

    [FirestoreProperty]
    public string? VerificationToken { get; set; }

    [FirestoreProperty]
    public Timestamp VerifiedAt { get; set; }
    
    [FirestoreProperty]
    public string PasswordReestToken { get; set; } = null!;

    [FirestoreProperty]
    public Timestamp ResetTokenExpires { get; set; }

    [FirestoreProperty]
    public string Avatar { get; set; } = null!;

    [FirestoreProperty]
    public bool IsGoogleAccount { get; set; } = false;

    public Role? Role { get; set; }

    [FirestoreProperty]
    public string? RoleId { get; set; }
}
