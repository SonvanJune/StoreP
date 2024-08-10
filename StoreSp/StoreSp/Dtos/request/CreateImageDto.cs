using System;
using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class CreateImageDto
{
    [Required]
    public required string Image { get; set;}
}
