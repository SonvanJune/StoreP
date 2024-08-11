using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class GetProductLikeDto
{
    [Required]
    public required int Page { get; set;}

    [Required]
    public required int ProductInPage { get; set;}

    [Required]
    public required string Username { get; set;}
}
