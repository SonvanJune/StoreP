using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public class UploadFilesDto
{
    [Required]
    public required List<IFormFile> files;
}
