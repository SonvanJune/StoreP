using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record UpdateCategoryDto
(
    [Required]
    string Code,
    [Required]
    string Name,
    [Required]
    string Avatar
);
