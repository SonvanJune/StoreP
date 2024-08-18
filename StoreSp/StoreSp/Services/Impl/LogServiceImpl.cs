
using System.Net;
using StoreSp.Commonds;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class LogServiceImpl : ILogService
{
    public static LogFireStore? LogFireStore { get; set; }
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
