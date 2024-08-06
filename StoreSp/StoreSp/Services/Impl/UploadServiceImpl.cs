using System.Net;
using BCrypt.Net;
using StoreSp.Commonds;
using StoreSp.Dtos.request;

namespace StoreSp.Services.Impl;

public class UploadServiceImpl : IUploadService
{
    private readonly string _uploadPath = @"D:\Uploads\Images";
    IResult IUploadService.UploadFiles(UploadFilesDto dto)
    {
        if (dto.files != null && dto.files.Count > 0)
        {
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Upload Sucess",
                data = Upload(dto.files).Result
            });
        }
        return Results.NoContent();
    }

    public async Task<String> Upload(List<IFormFile> files)
    {
        // Tạo thư mục nếu chưa tồn tại
        Directory.CreateDirectory(_uploadPath);
        string name = "";

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                name = BCrypt.Net.BCrypt.HashPassword(file.FileName);
                var filePath = Path.Combine(_uploadPath, name);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }
        return name;
    }
}
