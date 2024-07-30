using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;
using StoreSp.Stores.Stores;

namespace StoreSp.Stores;

public class LogFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionLog = "logs";
    public readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();

    public async Task<Log> AddLogForUser(User user, string code)
    {
        var db = _firestoreDb.Collection(_collectionLog);
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

        var request = new CreateLogDto
        {
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

    public List<LogDto> GetLogs()
    {
        var logDb = base.GetSnapshots(_collectionLog);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        List<LogDto> logDtos = new List<LogDto>();
        var logs = logDb.Documents.Select(r => r.ConvertTo<Log>()).ToList();
        foreach (var log in logs)
        {
            var logdto = new LogDto
            {
                Id = log.Id!,
                Code = log.Code!,
                Message = log.Message!,
                Status = log.Status
            };
            var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == log.UserId);
            var userDto = userConverter.ToDto(user!);
            logdto.User = userDto;
            logDtos.Add(logdto);
        }
        return logDtos;
    }
}
