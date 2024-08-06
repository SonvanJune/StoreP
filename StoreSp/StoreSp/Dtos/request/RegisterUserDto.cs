using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record RegisterUserDto
(
    [Required] string Name,
    string DeviceToken,

    string Email,

    string Phone,

    [Required] string Password,

    string Avatar,

    [Required] string RoleCode
);
