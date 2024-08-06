using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record TokenDto
(
    string Token,
    string RefreshToken
);
