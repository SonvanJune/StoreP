using Microsoft.EntityFrameworkCore;
using StoreSp.Context;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class SystemFirestore
{
    private AppDbContext? _appDbContext = null;

    public SystemFirestore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    public SystemDto GetCount(string username)
    {
        _appDbContext = AppDbContext.GetInstance();
        User shop = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
        {
            shop = _appDbContext!.Users.Include(r => r.Notifications).Include(r => r.Boxchats).SingleOrDefault(r => r.Phone == username)!;
        }
        else
        {
            shop = _appDbContext!.Users.Include(r => r.Notifications).Include(r => r.Boxchats).SingleOrDefault(r => r.Email == username)!;
        }

        if (shop == null)
        {
            return null!;
        }

        int countMessNotRead = 0;
        foreach (var item in shop.Boxchats!)
        {
            var boxchat = _appDbContext!.Boxchats.Include(r => r.Messages).SingleOrDefault(r => r.Id == item.Id);
            var countMessNotReadList = boxchat!.Messages!.ToList().FindAll(r => r.BoxchatId == boxchat.Id && r.Status == 0 && r.SenderId != shop.Id);
            if (countMessNotReadList.Count > 0 && countMessNotReadList != null)
            {
                countMessNotRead = countMessNotReadList.Count();
            }
        }

        int countNotifications = 0;
        foreach (var item in shop.Notifications!)
        {
            if (item.Status == 0)
            {
                countNotifications += 1;
            }
        }

        var result = new SystemDto
        {
            CountMessage = countMessNotRead,
            CountNotification = countNotifications
        };

        return result;
    }
}
