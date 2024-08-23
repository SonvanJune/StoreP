using Google.Cloud.Firestore;
using StoreSp.Context;
using StoreSp.Converters;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class LogFireStore
{
    public readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();

    private readonly AppDbContext? _appDbContext = null;

    public LogFireStore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    public async Task<Log> AddLogForUser(User user, string code)
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
            CreatedAt = DateTime.Now,
        };

        _appDbContext.Logs.Add(log);
        await _appDbContext.SaveChangesAsync();
        return log;
    }

    public List<LogDto> GetLogs()
    {
        List<LogDto> logDtos = new List<LogDto>();
        var logs = _appDbContext!.Logs.ToList();
        foreach (var log in logs)
        {
            var logdto = new LogDto
            {
                Id = log.Id,
                Code = log.Code!,
                CreatedAt = log.CreatedAt.ToString(),
                Message = log.Message!,
                Status = log.Status
            };
            var user = _appDbContext.Users.SingleOrDefault(r => r.Id == log.UserId);
            var userDto = userConverter.ToDto(user!);
            logdto.User = userDto;
            logDtos.Add(logdto);
        }
        return logDtos;
    }
}
