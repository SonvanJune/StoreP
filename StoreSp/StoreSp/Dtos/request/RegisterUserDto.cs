using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record RegisterUserDto
(
    [Required] string Name,

    string Email,

    string Phone,

    [Required] string Password,

    [Required] string Avatar,

    [Required] string RoleId
);
