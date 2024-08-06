using System.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;

namespace StoreSp.Services.Impl;

public class UploadServiceImpl : IUploadService
{
    private readonly string _uploadPath = @"D:\Uploads\Images";
    IResult IUploadService.UploadFiles(UploadFilesDto dto)
    {
        if (dto.Files != null && dto.Files.Count > 0)
        {
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Upload Sucess",
                data = null
            });
        }
        else
        {
            return Results.BadRequest(new HttpStatusConfig
            {
                status = HttpStatusCode.BadRequest,
                message = "Upload failed",
                data = null
            });
        }
    }

    public async Task<List<string>> Upload(List<IFormFile> files)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };

        // Tạo thư mục nếu chưa tồn tại
        Directory.CreateDirectory(_uploadPath);
        List<string> names = new List<string>();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return null!;
                }

                string name = BCrypt.Net.BCrypt.HashPassword(file.FileName);
                var filePath = Path.Combine(_uploadPath, name);
                names.Add(name);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }
        return names;
    }
}
