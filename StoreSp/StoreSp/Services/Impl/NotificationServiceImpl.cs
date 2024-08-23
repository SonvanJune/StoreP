

using System.Net;
using StoreSp.Commonds;
using StoreSp.Stores;

namespace StoreSp.Services.Impl;

public class NotificationServiceImpl : INotificationService
{
    public static NotificationFireStore NotificationFireStore = new NotificationFireStore();

    IResult INotificationService.DeleteAllNotifications(string username , int status)
    {
        var data = NotificationFireStore!.DeleteAllNotifications(username , status);
        if(data != null){
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Xóa thông báo thành công",
                data = null
            });
        }
        else{
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Không tìm thấy người dùng",
                data = null
            });
        }
    }

    IResult INotificationService.DeleteNotification(string notificationId)
    {
        var data = NotificationFireStore!.DeleteNotification(notificationId);
        if(data != null){
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Xóa thông báo thành công",
                data = null
            });
        }
        else{
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Không tìm thấy thông báo",
                data = null
            });
        }
    }

    IResult INotificationService.GetNotifications(string username , int status)
    {
        return Results.Ok(new HttpStatusConfig
        {
            status = HttpStatusCode.OK,
            message = "Thành công",
            data = NotificationFireStore!.GetNotifications(username , status)
        });
    }

    IResult INotificationService.MakeNotReadNotification(string notificationId)
    {
        var data = NotificationFireStore!.MakeNotReadNotification(notificationId);
        if(data != null){
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Chưa đọc thông báo thành công",
                data = null
            });
        }
        else{
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Không tìm thấy thông báo",
                data = null
            });
        }
    }

    IResult INotificationService.ReadALLNotification(string username)
    {
        var data = NotificationFireStore!.ReadALLNotification(username);
        if(data != null){
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Đọc thông báo thành công",
                data = null
            });
        }
        else{
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Không tìm thấy người dùng",
                data = null
            });
        }
    }

    IResult INotificationService.ReadNotification(string notificationId)
    {
        var data = NotificationFireStore!.ReadNotification(notificationId);
        if(data != null){
            return Results.Ok(new HttpStatusConfig
            {
                status = HttpStatusCode.OK,
                message = "Đọc thông báo thành công",
                data = null
            });
        }
        else{
            return Results.NotFound(new HttpStatusConfig
            {
                status = HttpStatusCode.NotFound,
                message = "Không tìm thấy thông báo",
                data = null
            });
        }
    }
}
