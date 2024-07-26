using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record GoogleLoginDto
(
    [Required] string Email,
    [Required] bool EmailVerified
);
