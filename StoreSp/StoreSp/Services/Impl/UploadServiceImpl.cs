using System.Net;
using System.Security.Cryptography;
using System.Text;
using StoreSp.Commonds;
using StoreSp.Dtos.request;

namespace StoreSp.Services.Impl;

public class UploadServiceImpl : IUploadService
{
    private readonly string _uploadPath = @"D:/Uploads/Images";
    IResult IUploadService.UploadFiles(UploadFilesDto dto)
    {
        if (dto.Files != null && dto.Files.Count > 0)
        {
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Upload Sucess",
                data = Upload(dto.Files).Result
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

    async Task<IResult> IUploadService.GetImagePhone(string imageName)
    {
        string imageUrl = _uploadPath + "/" + imageName;

        // Ensure the file path is valid and does not contain illegal characters
        if (Path.IsPathFullyQualified(imageUrl) && File.Exists(imageUrl))
        {
            var fileBytes = await File.ReadAllBytesAsync(imageUrl);
            var contentType = GetContentType(imageUrl); // Determine content type based on file extension

            return Results.File(fileBytes, contentType, Path.GetFileName(imageUrl));
        }
        else
        {
            return Results.NotFound("File not found.");
        }
    }

    async Task<IResult> IUploadService.GetImage(string imageName)
    {
        string imageUrl = _uploadPath + "/" + imageName;

        // Ensure the file path is valid and does not contain illegal characters
        if (Path.IsPathFullyQualified(imageUrl) && File.Exists(imageUrl))
        {
            var fileBytes = await File.ReadAllBytesAsync(imageUrl);
            var contentType = GetContentType(imageUrl); // Determine content type based on file extension

            return Results.File(fileBytes, contentType);
        }
        else
        {
            return Results.NotFound("File not found.");
        }
    }

    public string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            _ => "application/octet-stream" // Default content type for unknown file types
        };
    }
    public async Task<List<string>> Upload(IFormFileCollection files)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };

        // Tạo thư mục nếu chưa tồn tại
        Directory.CreateDirectory(_uploadPath);
        List<string> names = new List<string>();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var fileExtension = Path.GetExtension(file.FileName);

                if (!allowedExtensions.Contains(fileExtension.ToLowerInvariant()))
                {
                    return null!;
                }

                // Tạo hash MD5 cho tên file gốc
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var md5 = MD5.Create();
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(originalFileName));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                // Tạo tên file mới với phần mở rộng gốc
                var newFileName = $"{hashString}{fileExtension}";
                var filePath = Path.Combine(_uploadPath, newFileName);
                names.Add(newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }
        return names;
    }
}
