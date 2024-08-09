using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class NotificationFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionNotification = "notifications";

    public async Task<Notification> AddNotificationForUser(User user, string message , int type)
    {
        var db = _firestoreDb.Collection(_collectionNotification);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        User u = null!;
        if (user.Email == null)
        {
            u = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == user.Phone)!;
        }
        else
        {
            u = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == user.Email)!;
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
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
        };

        await db.AddAsync(notification);
        return notification;
    }
    
    public List<NotificationDto> GetNotifications(string username)
    {
        var notificationDb = base.GetSnapshots(_collectionNotification);
        List<NotificationDto> notificationDtos = new List<NotificationDto>();
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        User user;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username) != null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == username)!;
        }
        if (user == null)
        {
            return null!;
        }

        var notifications = notificationDb.Documents.Select(r => r.ConvertTo<Notification>()).ToList().FindAll(r => r.UserId == user.Id);
        foreach (var notification in notifications)
        {
            var notificationDto = new NotificationDto
            {
                Id = notification.Id!,
                CreatedAt = notification.CreatedAt.ToDateTime().ToString(),
                Message = notification.Message!,
                Status = notification.Status,
                Type = notification.Type
            };
            notificationDtos.Add(notificationDto);
        }
        return notificationDtos;
    }
}

