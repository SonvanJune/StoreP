using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record TokenDto
(
    [Required] string Token,
    [Required] string RefreshToken
);
