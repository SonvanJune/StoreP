using Google.Cloud.Firestore;
using StoreSp.Context;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class NotificationFireStore
{
    private readonly AppDbContext? _appDbContext = null;

    public NotificationFireStore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    public async Task<Notification> AddNotificationForUser(User user, string message, int type)
    {
        User u = null!;
        if (user.Email == null)
        {
            u = _appDbContext!.Users.SingleOrDefault(r => r.Phone == user.Phone)!;
        }
        else
        {
            u = _appDbContext!.Users.SingleOrDefault(r => r.Email == user.Email)!;
        }

        var request = new CreateNotificationDto
        {
            Message = message,
            Type = type
        };

        var notification = new Notification
        {
            UserId = u.Id,
            Type = request.Type,
            Message = request.Message,
            Status = 0,
            CreatedAt = DateTime.Now,
        };

        _appDbContext.Notifications.Add(notification);
        await _appDbContext.SaveChangesAsync();
        return notification;
    }

    public List<NotificationDto> GetNotifications(string username, int status)
    {
        List<NotificationDto> notificationDtos = new List<NotificationDto>();
        User user;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) != null)
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == username)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == username)!;
        }
        if (user == null)
        {
            return null!;
        }

        List<Notification> notifications = new List<Notification>();
        if (status == -1)
        {
            notifications = _appDbContext!.Notifications.Where(r => r.UserId == user.Id).ToList();
        }
        else
        {
            notifications = _appDbContext!.Notifications.Where(r => r.UserId == user.Id && r.Status == status).ToList();
        }

        foreach (var notification in notifications)
        {
            var notificationDto = new NotificationDto
            {
                Id = notification.Id,
                CreatedAt = notification.CreatedAt.ToString(),
                Message = notification.Message!,
                Status = notification.Status,
                Type = notification.Type
            };
            notificationDtos.Add(notificationDto);
        }
        return notificationDtos;
    }

    public async Task<string> ReadNotification(string notificationId)
    {
        var notification = _appDbContext!.Notifications.SingleOrDefault(r => r.Id == Convert.ToInt32(notificationId));
        if (notification == null)
        {
            return null!;
        }
        notification.Status = 1;
        _appDbContext.Notifications.Update(notification);
        await _appDbContext!.SaveChangesAsync();
        return "";
    }

    public async Task<string> DeleteNotification(string notificationId)
    {
        var notification = _appDbContext!.Notifications.SingleOrDefault(r => r.Id == Convert.ToInt32(notificationId));
        if (notification == null)
        {
            return null!;
        }
        _appDbContext.Notifications.Remove(notification);
        await _appDbContext!.SaveChangesAsync();
        return "";
    }

    public async Task<string> DeleteAllNotifications(string username, int status)
    {
        User user;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) != null)
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == username)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == username)!;
        }
        if (user == null)
        {
            return null!;
        }

        List<Notification> notifications = new List<Notification>();
        if (status == -1)
        {
            notifications = _appDbContext!.Notifications.Where(r => r.UserId == user.Id).ToList();
        }
        else
        {
            notifications = _appDbContext!.Notifications.Where(r => r.UserId == user.Id && r.Status == status).ToList();
        }

        foreach (var notification in notifications)
        {
            _appDbContext.Notifications.Remove(notification);
        }
        await _appDbContext!.SaveChangesAsync();
        return "";
    }

    public async Task<string> ReadALLNotification(string username)
    {
        User user;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) != null)
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == username)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == username)!;
        }
        if (user == null)
        {
            return null!;
        }

        var notifications = _appDbContext!.Notifications.Where(r => r.UserId == user.Id).ToList();

        foreach (var notification in notifications)
        {
            notification.Status = 1;
            _appDbContext.Notifications.Update(notification);
        }
        await _appDbContext!.SaveChangesAsync();
        return "";
    }

    public async Task<string> MakeNotReadNotification(string notificationId)
    {
        var notification = _appDbContext!.Notifications.SingleOrDefault(r => r.Id == Convert.ToInt32(notificationId));
        if (notification == null)
        {
            return null!;
        }
        notification.Status = 0;
        _appDbContext.Notifications.Update(notification);
        await _appDbContext!.SaveChangesAsync();
        return "";
    }
}

