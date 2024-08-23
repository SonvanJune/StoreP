namespace StoreSp.Models;

public class User
{
    public int Id { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.Now;

    public DateTime UpdateAt { get; set; } = DateTime.Now;

    public required string Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public required int Account { get; set; }

    public string? PasswordHash { get; set; }

    public int Status { get; set; }

    public string? VerificationToken { get; set; }

    public string? DeviceToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime VerifiedAt { get; set; }
    
    public string? PasswordReestToken { get; set; }

    public DateTime ResetTokenExpires { get; set; }

    public string? Avatar { get; set; }

    public int IsGoogleAccount { get; set; }
    public int IsUpdated { get; set; }

    public Role? Role { get; set; }

    public int RoleId { get; set; }

    public Cart? Cart { get; set; }

    public ICollection<Notification>? Notifications { get; set;}
    public ICollection<Product>? ProductSells { get; set;}
    public ICollection<Log>? Logs { get; set;}
    public ICollection<Address>? Addresses { get; set;}
    public ICollection<Bill>? Bills { get; set; }
    public ICollection<Like>? Likes { get; set; }
    public ICollection<Message>? ReceivMessages { get; set; }
    public ICollection<Message>? SendMessages { get; set; }
    public ICollection<Boxchat>? Boxchats { get; set; }
}
