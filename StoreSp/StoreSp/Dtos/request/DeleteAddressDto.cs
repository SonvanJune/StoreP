using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class DeleteAddressDto
{
    [Required]
    public required List<string> Codes { get; set;}
}
