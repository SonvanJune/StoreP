using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class AddBannerDto
{
    [Required]
    public required List<string> Images { get; set;}
}
