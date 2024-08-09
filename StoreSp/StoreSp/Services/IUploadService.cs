using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IUploadService
{
    IResult UploadFiles(UploadFilesDto files);

    Task<IResult> GetImage(string imageName);
    Task<IResult> GetImagePhone(string imageName);
}
