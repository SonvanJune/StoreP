using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record LoginUserDto(
    [Required] string Email,
    [Required] string Password
);