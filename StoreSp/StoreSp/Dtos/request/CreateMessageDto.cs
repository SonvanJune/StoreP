using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateMessageDto
{
    [Required]
    public required string Receiver { get; set; }

    [Required]
    public required string Message { get; set; }

    [Required]
    public required string BoxchatCode { get; set; }
}
