
namespace StoreSp.Services;

public interface INotificationService
{
    public IResult GetNotifications(string username);
    public IResult DeleteNotification(string notificationId);
    public IResult DeleteAllNotifications(string username);
    public IResult ReadNotification(string notificationId);
}
