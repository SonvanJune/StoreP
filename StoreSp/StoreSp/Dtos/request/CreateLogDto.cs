using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateLogDto
{
    [Required]
    public required string Code { get; set;}

    [Required]
    public required string Message { get; set;}
}
