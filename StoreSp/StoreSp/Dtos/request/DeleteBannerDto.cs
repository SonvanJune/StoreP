using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class DeleteBannerDto
{
    [Required]
    public required List<int> Ids { get; set;}
}
