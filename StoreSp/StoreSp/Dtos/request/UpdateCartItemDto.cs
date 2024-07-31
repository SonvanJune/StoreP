using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record UpdateCartItemDto(
    [Required] string ItemCode,
    [Required] int Quantity,
    List<string> ClassifyCodes
);
