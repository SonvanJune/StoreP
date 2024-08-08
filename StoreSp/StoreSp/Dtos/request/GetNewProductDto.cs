using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class GetNewProductDto
{
    [Required]
    public required int Page { get; set;}

    [Required]
    public required int ProductInPage { get; set;}

    [Required]
    public required int Day { get; set;}
}
