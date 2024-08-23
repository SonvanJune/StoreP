
using System.Net;
using StoreSp.Commonds;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class LogServiceImpl : ILogService
{
    public static LogFireStore LogFireStore  = new LogFireStore();
    IResult ILogService.GetLogs()
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = LogFireStore!.GetLogs()
        });
    }
}
