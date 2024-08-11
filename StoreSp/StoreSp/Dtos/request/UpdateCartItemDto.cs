using System.ComponentModel.DataAnnotations;
using StoreSp.Dtos.response;

namespace StoreSp.Dtos.request;

public record UpdateCartItemDto(
    [Required] string ItemCode,
    string Quantity,
    List<string> ClassifyCodes
);
