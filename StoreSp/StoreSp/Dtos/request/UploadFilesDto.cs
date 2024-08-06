using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class UploadFilesDto
{
    [Required]
    public required IFormFileCollection Files { get; set; }
}
