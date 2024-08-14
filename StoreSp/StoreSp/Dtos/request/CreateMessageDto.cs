using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateMessageDto
{
    [Required]
    public required string Sender { get; set; }

    [Required]
    public required string Message { get; set; }
}
