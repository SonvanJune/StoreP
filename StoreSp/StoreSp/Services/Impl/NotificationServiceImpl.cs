

using System.Net;
using StoreSp.Commonds;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class NotificationServiceImpl : INotificationService
{
    public static NotificationFireStore? NotificationFireStore { get; set; }
    IResult INotificationService.GetNotifications(string username)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "success",
            data = NotificationFireStore!.GetNotifications(username)
        });
    }
}
