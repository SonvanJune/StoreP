using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class GetProductLikeDto
{
    [Required]
    public required string Username { get; set;}
}
