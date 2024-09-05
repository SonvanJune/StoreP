using StoreSp.Context;

namespace StoreSp.Stores;

public class SystemFirestore
{
    private readonly AppDbContext? _appDbContext = null;

    public SystemFirestore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    
}
