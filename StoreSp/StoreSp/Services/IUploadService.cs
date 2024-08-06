using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IUploadService
{
    IResult UploadFiles(UploadFilesDto files);
}
