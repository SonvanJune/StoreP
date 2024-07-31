namespace StoreSp.Dtos.response;

public class ShippingMethodDto
{
    public required string Name { get; set; }
    public required string CreatedAt { get; set; }
    public required string Code { get; set; }
    public required string Location { get; set; }
    public required int Ensure { get; set; }
    public required int Price { get; set; }
    public required int Status { get; set; }
}
