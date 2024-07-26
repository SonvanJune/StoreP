namespace StoreSp.Dtos.response;

public class ProductClassifyDto
{
    public required string GroupName { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Image { get; set; }
    public required int Quantity { get; set; }
    public required int IncreasePercent { get; set; }
    public int PriceAfterIncreasePercent { get; set; }
    public required int Status { get; set; }
}
