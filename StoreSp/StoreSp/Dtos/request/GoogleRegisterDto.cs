using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record GoogleRegisterDto
(
    [Required] string Name,

    string Email,

    string Avatar,

    [Required] string RoleCode,
    [Required] bool EmailVerified
);
