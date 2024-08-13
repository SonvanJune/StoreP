using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateBoxchatDto
{
    [Required]
    public required string Username { get; set; }
}
