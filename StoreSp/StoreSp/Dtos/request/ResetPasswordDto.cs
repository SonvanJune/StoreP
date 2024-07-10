using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class ResetPasswordDto
{
    [Required]
    public string Code { get; set; } = string.Empty;

    [Required, MinLength(6,ErrorMessage ="Please enter more than 6")]
    public string Password { get; set; } = string.Empty;
    
    [Required, Compare("Password")]
    public string ComfirmPassword { get; set; } = string.Empty;    
}
