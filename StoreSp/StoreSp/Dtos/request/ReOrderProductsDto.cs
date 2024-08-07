using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class ReOrderProductsDto
{
    [Required] public required string Code {get; set; }
}
