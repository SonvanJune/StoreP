using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateRoleDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Code { get; set; }
}
