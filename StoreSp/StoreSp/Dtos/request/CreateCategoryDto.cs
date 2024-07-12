using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateCategoryDto
{
    [Required]
    public required string Name { get; set; }
}
