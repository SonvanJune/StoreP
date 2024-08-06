using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record LoginUserDto(
    [Required] string Username,
    [Required] string Password,
    [Required] string DeviceToken
);