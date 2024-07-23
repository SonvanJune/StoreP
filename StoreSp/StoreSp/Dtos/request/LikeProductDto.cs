using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record LikeProductDto
(
    [Required]
    string Username,
    [Required]
    string ProductCode
);
