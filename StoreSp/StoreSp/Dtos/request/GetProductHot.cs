using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class GetProductHot
{
    [Required]
    public required int Page { get; set;}

    [Required]
    public required int ProductInPage { get; set;}
}
