using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateNotificationDto
{

    [Required]
    public required string Message { get; set; }

    [Required]
    public required int Type { get; set; }
}
