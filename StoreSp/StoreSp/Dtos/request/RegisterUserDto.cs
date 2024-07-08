using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record RegisterUserDto
(
    [Required] string Name,

    [Required] string Email,

    [Required] string Phone,

    [Required] string Password,

    [Required] string Avatar,

    [Required] string RoleId
);
