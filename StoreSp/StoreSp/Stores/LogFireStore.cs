using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class LogFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionLog = "logs";

    public async Task<Log> AddLogForUser(User user , string code){
        var db = _firestoreDb.Collection(_collectionLog);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        User u = null!;
        if (user.Email == null)
        {
            u = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == user.Phone)!;
        }
        else{
            u = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == user.Email)!;
        }

        var request = new CreateLogDto{
            Code = code,
            Message = $"{u.Id} - {code}"
        };

        var log = new Log
        {
            UserId = u.Id,
            Code = request.Code,
            Message = request.Message,
            Status = 0,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
        };

        await db.AddAsync(log);
        return log;
    }
}
