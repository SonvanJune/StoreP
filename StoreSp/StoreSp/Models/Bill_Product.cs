namespace StoreSp.Models;

public class Bill_Product
{
    public required int ProductId { get; set; }

    public required int BillId { get; set; }

    public required int Quantity { get; set; }

    public Product? Product { get; set; }
    public Bill? Bill { get; set; }

    public string? ProductClassifies { get; set; }
}
