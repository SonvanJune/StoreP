namespace StoreSp.Dtos.response;

public class BillItemDto
{
    public int Quantity { get; set; }
    public string? ProductClassifies { get; set; }
    public ProductDto? Product { get; set; }
}
