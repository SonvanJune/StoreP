namespace StoreSp.Dtos.request;

public class GetBillOfUserDto
{
    public required string Username { get; set; }
    public string? Status { get; set; }
}
