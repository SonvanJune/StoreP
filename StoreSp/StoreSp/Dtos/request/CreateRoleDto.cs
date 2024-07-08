using System.ComponentModel.DataAnnotations;

namespace StoreSp;

public class CreateRoleDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Code { get; set; }
}
