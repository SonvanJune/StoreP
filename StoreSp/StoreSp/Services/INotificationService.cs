
namespace StoreSp.Services;

public interface INotificationService
{
    public IResult GetNotifications(string username , int status);
    public IResult DeleteNotification(string notificationId);
    public IResult DeleteAllNotifications(string username, int status);
    public IResult ReadNotification(string notificationId);
}
