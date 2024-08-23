namespace StoreSp.Models;

public class Address
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public required string Description { get; set; }

    public required string Location { get; set; }

    public required string Long { get; set; }

    public required string Lat { get; set; }

    public required string PhoneGet { get; set; }

    public required string NameGet { get; set; }

    public required string Status { get; set; }
    public ICollection<User>? Users { get; set;}

    public ICollection<Bill>? Bills { get; set; }
}
