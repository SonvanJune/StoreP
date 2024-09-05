using StoreSp.Context;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class SystemFirestore
{
    private readonly AppDbContext? _appDbContext = null;

    public SystemFirestore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    // public Task<SystemDto> GetCount(string username){
        
    //     User shop = null!;
    //     if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
    //     {
    //         shop = _appDbContext!.Users.Include(r => r.ProductSells).SingleOrDefault(r => r.Phone == username)!;
    //     }
    //     else
    //     {
    //         shop = _appDbContext!.Users.Include(r => r.ProductSells).SingleOrDefault(r => r.Email == username)!;
    //     }

    //     if (shop == null)
    //     {
    //         return null!;
    //     }

    // }
}
