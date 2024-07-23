using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class ResetCodeDto
{
    [Required] public string? Code {get; set; }
}
